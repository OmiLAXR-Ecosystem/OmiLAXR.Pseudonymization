/*
* SPDX-License-Identifier: AGPL-3.0-or-later
* Copyright (C) 2025 Sergej Görzen <sergej.goerzen@gmail.com>
* This file is part of OmiLAXR.Pseudonymization.
*/
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace OmiLAXR.Pseudonymization.Helper
{
    /// <summary>
    /// Minimal HTTP access needed for fetching pseudonyms. Replace it to test without network.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>Returns the response body, or null if the request failed.</summary>
        Task<string> GetStringAsync(string uri);
    }

    /// <summary>
    /// <see cref="IHttpClient"/> based on <see cref="UnityWebRequest"/>.
    /// </summary>
    public sealed class UnityWebRequestHttpClient : IHttpClient
    {
        public static readonly UnityWebRequestHttpClient Instance = new UnityWebRequestHttpClient();

        public async Task<string> GetStringAsync(string uri)
        {
            using (var request = UnityWebRequest.Get(uri))
            {
                var asyncOp = request.SendWebRequest();
                while (!asyncOp.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                    return null;
                }

                return request.downloadHandler.text;
            }
        }
    }
}
