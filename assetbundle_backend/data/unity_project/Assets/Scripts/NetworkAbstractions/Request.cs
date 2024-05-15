using System;
using NetworkAbstractions.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkAbstractions
{
    public static class Request
    {
        private const string BASE_URL = "https://users.sussex.ac.uk/~vk234/Web3D/assetbundle_backend/";
        private const int MIN_SERVER_VER = 1;

        public static void DoRequest(string method, Action<UnityWebRequestAsyncOperation> success,
            Action<UnityWebRequestAsyncOperation> failure, RequestType requestType = RequestType.GET,
            byte[] data = null)
        {
            // Check version before any requests!
            DoOperationIfServerVersionCorrect(() =>
            {
                InternalDoRequest(method, success, failure, requestType, data);
            }, $"request to {method}");
        }

        private static void DoOperationIfServerVersionCorrect(Action operation, string description)
        {
            CheckVersion(correctVersion =>
            {
                if (correctVersion)
                {
                    operation.Invoke();
                    return;
                }
                Debug.LogWarning($"{description} aborted - server ver incorrect");
            });
        }

        public static void LoadDataFromServer<T>(string method, Action<T> success,
            Action<UnityWebRequestAsyncOperation> failure)
        {
            DoOperationIfServerVersionCorrect(() =>
            {
                InternalDoRequest(method, operation =>
                {
                    success.Invoke(JsonConvert.DeserializeObject<T>(operation.webRequest.downloadHandler.text));
                }, failure);
            }, $"request to {method}");
        }

        public static void LoadBundle(string path, Action<AssetBundle> success, Action<UnityWebRequestAsyncOperation> failure)
        {
            // Check version before any requests!
            DoOperationIfServerVersionCorrect(() =>
                {
                    var bundleReq = UnityWebRequestAssetBundle.GetAssetBundle($"{BASE_URL}/{path}");
                    BindAndSendWebRequest(bundleReq, operation =>
                    {
                        success.Invoke(DownloadHandlerAssetBundle.GetContent(operation.webRequest));
                    }, failure);
                }, $"AssetBundle request to {path}");
            
        }
        
        private static void CheckVersion(Action<bool> callback)
        {
            InternalDoRequest("version.php", operation =>
            {
                var version = JsonUtility.FromJson<ServerVersion>(operation.webRequest.downloadHandler.text);
                callback.Invoke(version.version >= MIN_SERVER_VER);
            }, operation =>
            {
                Debug.LogError($"Getting server version failed - {operation.webRequest.error}");
                callback.Invoke(false);
            });
        }

        private static void InternalDoRequest(string method, Action<UnityWebRequestAsyncOperation> success,
            Action<UnityWebRequestAsyncOperation> failure, RequestType requestType = RequestType.GET, byte[] data = null)

        {
            UnityWebRequest req;
            var path = $"{BASE_URL}{method}";
            switch (requestType)
            {
                case RequestType.GET:
                    if (data != null)
                    {
                        throw new ArgumentException("data not supported for GET");
                    }
                    req = UnityWebRequest.Get(path);
                    break;
                case RequestType.HEAD:
                    if (data != null)
                    {
                        throw new ArgumentException("data not supported for HEAD");
                    }
                    req = UnityWebRequest.Head(path);
                    break;
                case RequestType.PUT:
                    data ??= Array.Empty<byte>();
                    req = UnityWebRequest.Put(path, data);
                    break;
                case RequestType.DELETE:
                    data ??= Array.Empty<byte>();
                    req = UnityWebRequest.Put(path, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
            }
            BindAndSendWebRequest(req, success, failure);
        }



        private static void BindAndSendWebRequest(UnityWebRequest req, Action<UnityWebRequestAsyncOperation> success,
            Action<UnityWebRequestAsyncOperation> failure)
        {
            // Report to the Editor progress utility if in Editor! Helps for debugging
#if UNITY_EDITOR
            var progId = Progress.Start($"[Network] {req.uri}", options: Progress.Options.Indefinite);
#endif
            var op = req.SendWebRequest();
            op.completed += _ =>
            {
                Debug.Log(op);
                if (op.webRequest.result == UnityWebRequest.Result.Success && op.webRequest.responseCode == 200)
                {
#if UNITY_EDITOR
                    Progress.Finish(progId);
#endif
                    success.Invoke(op);
                    return;
                }

#if UNITY_EDITOR
                Progress.SetDescription(progId, $"failed - {req.error}");
                Progress.Finish(progId, Progress.Status.Failed);
#endif
                failure.Invoke(op);
            };
        }
    }
}