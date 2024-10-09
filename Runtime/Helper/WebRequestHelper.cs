using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace OmiLAXR.Pseudonymization.Helper
{
    public static class WebRequestHelper
    {
        public static IEnumerator GetRequest(string uri, Action<long, DownloadHandler> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                var pages = uri.Split('/');
                var page = pages.Length - 1;

                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    DebugLog.OmiLAXR.Error(pages[page] + ": Network Error: " + webRequest.error);
                    callback(webRequest.responseCode, null);
                }
                else if (webRequest.isDone)
                {
                    DebugLog.OmiLAXR.Print(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    callback(webRequest.responseCode, webRequest.downloadHandler);
                }
            }
        }
        
        public static async Task<T> GetJsonRequestAsync<T>(string uri)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                var asyncOp = request.SendWebRequest();

                // Await the completion of the request
                while (!asyncOp.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                    return default;
                }
                else
                {
                    return JsonUtility.FromJson<T>(request.downloadHandler.text);
                }
            }
        }


        public static IEnumerator GetJsonRequest<T>(string uri, Action<long, T> callback)
        {
            return GetRequest(uri, (code, downloadHandler) => {
                if (downloadHandler == null)
                    return;
                var jsonResult = JsonConvert.DeserializeObject<T>(downloadHandler.text);
                callback(code, jsonResult);
            });
        }
    }

}