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

        public string url => UriHelper.CombineToString(baseUrl, languageCode);
        
        protected override async Task<string> GenerateNewPseudonym()
        {
            // Create URI according to https://mtlg.elearn.rwth-aachen.de/pseudo/
            var qb = new QueryBuilder(url);
            qb.Append("capitalize", capitalize);

            var uri = qb.ToString();

            var pseudonymResponse = await WebRequestHelper.GetJsonRequestAsync<PseudonymResponse>(uri);
    
            return pseudonymResponse.pseudonym;
        }

    }
}