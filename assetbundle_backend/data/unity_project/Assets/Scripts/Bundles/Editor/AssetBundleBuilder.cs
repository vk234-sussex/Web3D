using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetworkAbstractions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Bundles.Editor
{
    public class AssetBundleBuilder : EditorWindow
    {
        private const string AssetBundleDirectory = "Assets/AssetBundles";
        
        private bool _redrawRequired = true;
        private AssetBundleManifest _manifest;
        
        [MenuItem("Window/Web3D/Web3D AssBun Builder")]
        public static void ShowWindow()
        {
            AssetBundleBuilder window = GetWindow<AssetBundleBuilder>();
            window.titleContent = new GUIContent("AssBunBuilder");
        }

        /// <summary>
        /// Checks if data needs to be changed (e.g after a build or dropdown change!)
        /// <seealso cref="Draw"/>
        /// </summary>
        private void OnGUI()
        {
            if (!_redrawRequired) return;
            Draw();
            // Draw is garbage generating, so cleanup after ourselves
            // The for loop here is a hack for when inc. GC is enabled
            for (var i = 0; i < 6; i++) GC.Collect();
            // Mark as having finished!
            _redrawRequired = false;
        }
        
        // UI Setup methods

        private void AddJoke(VisualElement root)
        {
            AddLabel(root, "this window brought to you by a certain ticket assigned to a man down under 🫡🖤\n\n");
            AddLabel(root, "Web3D export tool (C) Val Knight 2024");
        }

        private void AddLabel(VisualElement container, string labelText, float fontSize = 12f, FontStyle fontStyle = FontStyle.Normal, Align alignment = Align.Auto, StyleColor? color = null)
        {
            if (color == null)
            {
                color = new StyleColor(Color.black);
            }
            var label = new Label(labelText)
            {
                style =
                {
                    fontSize = fontSize,
                    unityFontStyleAndWeight = fontStyle,
                    alignSelf = alignment,
                    color = color.Value
                }
            };
            container.Add(label);
        }

        private void AddButtonWithCallback(VisualElement container, string buttonName, string buttonText,
            Action callback, StyleColor? backgroundColor = null, StyleColor? color = null, float fontSize = 12f, FontStyle fontStyle = FontStyle.Normal, float padding = 0f)
        {
            // Set default values
            backgroundColor ??= new StyleColor(Color.white);
            color ??= new StyleColor(Color.black);
            
            var button = new Button
            {
                name = buttonName,
                text = buttonText,
                style =
                {
                    backgroundColor = backgroundColor.Value,
                    color = color.Value,
                    fontSize = fontSize,
                    unityFontStyleAndWeight = fontStyle,
                    paddingBottom = padding,
                    paddingLeft = padding,
                    paddingRight = padding,
                    paddingTop = padding
                }
            };
            button.clicked += callback.Invoke;
            container.Add(button);
        }
        
        private void Draw()
        {
            VisualElement root = rootVisualElement;
            // Clear any existing text!
            root.Clear();
            AddJoke(root);
            
            // Add the build button!
            AddLabel(root, "Web3D Build System\n", 24f, FontStyle.Bold, Align.Center);
            AddButtonWithCallback(root, "main_build", "Build bundles", () =>
            {
                _manifest = BuildAllAssetBundles();
                // Mark as requiring redraw when complete! this is due to building having completed
                _redrawRequired = true;
            }, fontSize: 24f, backgroundColor: new StyleColor(Color.green), padding:4f, fontStyle: FontStyle.Bold);
            
            if (ReferenceEquals(_manifest, null))
            {
                AddLabel(root, "no manifest!! please build first!", fontStyle: FontStyle.Italic, color: new StyleColor(Color.red), alignment: Align.Center);
                return;
            }
            
            AddLabel(root, "\nBundles:", fontStyle: FontStyle.Bold);
            foreach (var bundle in _manifest.GetAllAssetBundles())
            {
                AddLabel(root, $"- {AssetBundleDirectory}/{bundle}");
            }
            AddButtonWithCallback(root, "main_deploy", "Deploy bundles", () =>
            {
                var confirmation = EditorUtility.DisplayDialog("Confirm deploy",
                    "This will deploy the bundles to the live build. Make sure you want to do this!", "lgtm 🚀", "cancel");
                if (!confirmation)
                {
                    Debug.Log("Aborting deploy");
                    return;
                }
                Debug.Log("Starting deploy process. Check the progress tab of Unity for details");
                Dictionary<string, string> bundlesToServerLocation = new Dictionary<string, string>();
                int remaining = _manifest.GetAllAssetBundles().Length;
                foreach (var bundle in _manifest.GetAllAssetBundles())
                {
                    var path = $"{AssetBundleDirectory}/{bundle}";
                    Request.DoRequest($"upload/upload.php?filename={bundle}", BundleUploadSuccess, BundleUploadFailure, RequestType.PUT, ReadCurrentBundle());
                    
                    continue;
                    
                    byte[] ReadCurrentBundle()
                    {
                        var file = System.IO.File.OpenRead(path);
                        byte[] data = new byte[file.Length];
                        var read = file.Read(data, 0, (int) file.Length);
                        Debug.Log($"Read {read} bytes");
                        return data;
                    }

                    void BundleUploadFailure(UnityWebRequestAsyncOperation operation)
                    {
                        Debug.Log($"failed to upload {path} - {operation.webRequest.error}");
                        remaining--;
                        if (remaining == 0)
                        {
                            UploadMapping(bundlesToServerLocation);
                        }
                    }

                    void BundleUploadSuccess(UnityWebRequestAsyncOperation operation)
                    {
                        bundlesToServerLocation[bundle] = operation.webRequest.downloadHandler.text;
                        Debug.Log($"Successfully uploaded - {path} -> {operation.webRequest.downloadHandler.text}");
                        remaining--;
                        if (remaining == 0)
                        {
                            UploadMapping(bundlesToServerLocation);
                        }
                    }
                }
            }, fontSize: 24f, backgroundColor: new StyleColor(Color.cyan), padding: 1f, fontStyle: FontStyle.Bold);
        }

        private void UploadMapping(Dictionary<string, string> bundlesToServerLocation)
        {
            Request.DoRequest($"upload/upload_manifest.php",
                operation => { Debug.Log("Successfully uploaded manifest"); },
                operation => { Debug.Log($"failed to upload manifest - {operation.webRequest.error}"); },
                RequestType.PUT, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bundlesToServerLocation)));
        }

        private void CreateGUI()
        {
            Draw();
        }
        AssetBundleManifest BuildAllAssetBundles()
        {
            
            if(!Directory.Exists(AssetBundleDirectory))
            {
                Directory.CreateDirectory(AssetBundleDirectory);
            }
            return BuildPipeline.BuildAssetBundles(AssetBundleDirectory, 
                BuildAssetBundleOptions.None, 
                BuildTarget.WebGL);
        }
    }
}
