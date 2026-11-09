using System;
using System.Threading.Tasks;
using OmiLAXR.Utils;
using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    public delegate void PseudonymBehaviourEvent(string pseudonym);
    public abstract class PseudonymBehaviour : MonoBehaviour
    {
        public event PseudonymBehaviourEvent OnCreatedPseudonym;

        public bool generateOnStartup = true;
        public bool rememberPseudonym = false;

        private IKeyValueStore _store = PlayerPrefsStore.Instance;

        /// <summary>
        /// Where remembered pseudonyms are stored (default: PlayerPrefs). Setting null restores the default.
        /// </summary>
        public IKeyValueStore Store
        {
            get => _store;
            set => _store = value ?? PlayerPrefsStore.Instance;
        }

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
            return Store.HasKey("pseudonym") ? Store.GetString("pseudonym") : null;
        }

        public void SavePseudonym(string pseudonym)
        {
            Store.SetString("pseudonym", pseudonym);
            Store.Save();
        }
    }
}