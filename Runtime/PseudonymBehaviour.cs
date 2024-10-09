using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    public delegate void PseudonymBehaviourEvent(string pseudonym);
    public abstract class PseudonymBehaviour : MonoBehaviour
    {
        protected static PseudonymBehaviour _instance;
        
        public event PseudonymBehaviourEvent OnCreatedPseudonym;

        public bool generateOnStartup = true;
        public bool rememberPseudonym = false;

        private void OnEnable()
        {
            // do nothing
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                if (generateOnStartup)
                    Generate();
                
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void Generate()
        {
            var rememberedName = LoadPseudonym();
            if (rememberPseudonym && rememberedName != null)
            {
                FireCreatedPseudonym(rememberedName);
                return;
            }
            
            var pseudonym = await GenerateNewPseudonym();
            FireCreatedPseudonym(pseudonym);

            // remember pseudonym if enabled
            if (rememberPseudonym)
            {
                SavePseudonym(pseudonym);
            }
        }

        protected abstract Task<string> GenerateNewPseudonym();
        
        protected void FireCreatedPseudonym(string pseudonym)
        {
            OnCreatedPseudonym?.Invoke(pseudonym);
        }
        
        public string LoadPseudonym()
        {
            return PlayerPrefs.HasKey("pseudonym") ? PlayerPrefs.GetString("pseudonym") : null;
        }

        public void SavePseudonym(string pseudonym)
        {
            PlayerPrefs.SetString("pseudonym", pseudonym);
            PlayerPrefs.Save();
        }
    }
}