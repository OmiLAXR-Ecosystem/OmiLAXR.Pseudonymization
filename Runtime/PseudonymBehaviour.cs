using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    public delegate void PseudonymBehaviourEvent(string pseudonym);
    public abstract class PseudonymBehaviour : MonoBehaviour
    {
        public event PseudonymBehaviourEvent OnCreatedPseudonym;

        public bool generateOnStartup = true;
        public bool rememberPseudonym = false;

        private void OnEnable()
        {
            // do nothing
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