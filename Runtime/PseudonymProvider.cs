using System;
using System.Threading.Tasks;
using OmiLAXR.Pseudonymization.Helper;
using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    /// <summary>
    /// Is generating an 100%-safe-unique pseudonym by using the web service https://mtlg.elearn.rwth-aachen.de/pseudo/. 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("OmiLAXR / Pseudonymization / Pseudonym Provider (MTLG)")]
    public class PseudonymProvider : PseudonymBehaviour
    {
        public static PseudonymProvider Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                if (generateOnStartup)
                    Generate();
                
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private struct PseudonymResponse
        {
            public string pseudonym;
        }

        public string baseUrl = "https://mtlg.elearn.rwth-aachen.de/pseudo/pseudonym/";
        public string languageCode = "en";
        public bool capitalize = true;

        private void Reset()
        {
            rememberPseudonym = true;
        }

        private IHttpClient _httpClient = UnityWebRequestHttpClient.Instance;

        /// <summary>
        /// HTTP client used to fetch pseudonyms (default: UnityWebRequest). Setting null restores the default.
        /// </summary>
        public IHttpClient HttpClient
        {
            get => _httpClient;
            set => _httpClient = value ?? UnityWebRequestHttpClient.Instance;
        }

        public string url => UriHelper.CombineToString(baseUrl, languageCode);
        
        protected override async Task<string> GenerateNewPseudonym()
        {
            // Create URI according to https://mtlg.elearn.rwth-aachen.de/pseudo/
            var qb = new QueryBuilder(url);
            qb.Append("capitalize", capitalize);

            var uri = qb.ToString();

            var json = await HttpClient.GetStringAsync(uri);
            return json == null ? null : JsonUtility.FromJson<PseudonymResponse>(json).pseudonym;
        }

    }
}