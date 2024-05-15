using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetworkAbstractions;
using Rendering;
using Unity.VisualScripting;
using UnityEngine;

namespace Bundles
{
    public class LoadFromBundle: MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void MarkAsReadyToLoad();
        [DllImport("__Internal")]
        private static extern void MarkAsLoading();
        [DllImport("__Internal")]
        private static extern void MarkLoaded();

        private Dictionary<string, AssetBundle> bundleCache = new();
        
        public bool loadOnBoot;
        public string bootServerId = "model_coke";
        public string assetName = "Prefab";
        public Material studioMat;
        public float transitionDuration = 0.3f;
        private GameObject modelInstance;
        #if UNITY_EDITOR
        [Header("This data is not included in builds! Use this for debugging materials in Editor")]
        [SerializeField] private Material editorMat;
        private Color _startColor;
        #endif

        private IEnumerator SetColour(Color color)
        {
            var sTime = Time.time;
            Color startColor = studioMat.color;
            while (Time.time - sTime < transitionDuration)
            {
                studioMat.color = Color.Lerp(startColor, color, (Time.time - sTime) / transitionDuration);
                yield return null;
            }
            studioMat.color = color;
        }
        public void Start()
        {
            MarkAsReadyToLoad();
            ResetScene();
#if UNITY_EDITOR
            _startColor = studioMat.color;
#endif
            if (loadOnBoot) LoadModelBundleWithId(bootServerId);
        }

        public void ResetScene()
        {
            if (modelInstance != null)
            {
                Destroy(modelInstance);
                modelInstance = null;
            }
            StartCoroutine(SetColour(Color.black));
        }

        public void LoadModelBundleWithId(string id)
        {
            MarkAsLoading();
            ResetScene();
            if (bundleCache.TryGetValue(id, out var bundle))
            {
                InstantiateBundle(bundle);
                return;
            }
            StartAssetBundleLoad(id, loadedBundle =>
            {
                bundleCache[id] = loadedBundle;
                InstantiateBundle(loadedBundle);
            });
        }

        private void InstantiateBundle(AssetBundle bundle)
        {
            MarkLoaded();
            bundle.LoadAllAssets();
            var prefab = bundle.LoadAsset<GameObject>(assetName);
            modelInstance = Instantiate(prefab, transform);
            modelInstance.name = "InstantiatedObject";
            // Change the studio colour
            StudioData[] sData = bundle.LoadAllAssets<StudioData>();
            if (sData.Length > 0)
            {
                StartCoroutine(SetColour(sData[0].color));
            }
#if UNITY_EDITOR
            if (modelInstance.TryGetComponent(out MeshRenderer _r))
            {
                _r.sharedMaterial = editorMat;
            }
#endif
        }
        
        private void StartAssetBundleLoad(string bundleId, Action<AssetBundle> callback) {
            Request.LoadDataFromServer<Dictionary<string, string>>("manifest.php", manifest =>
            {
                LoadWithServerManifest(bundleId, manifest, callback);
            }, operation =>
            {
                MarkLoaded();
                Debug.LogError($"failed to get server manifest - {operation.webRequest.error}");
            });
        }

        private void LoadWithServerManifest(string bundleId, Dictionary<string, string> serverManifest, Action<AssetBundle> callback)
        {
            if (!serverManifest.TryGetValue(bundleId, out var serverPath))
            {
                MarkLoaded();
                Debug.LogError($"{bundleId} not in server manifest!");
                return;
            }
            Request.LoadBundle(serverPath, callback, operation =>
            {
                Debug.Log($"failed to get {serverPath} - {operation.webRequest.error}");
                MarkLoaded();
            });
        }
#if UNITY_EDITOR
        public void OnDestroy()
        {
            studioMat.color = _startColor;
        }
#endif
    }
}