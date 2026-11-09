using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    /// <summary>
    /// Debug helper that logs every pseudonym the assigned pseudonym behaviour generates.
    /// </summary>
    [AddComponentMenu("OmiLAXR / Pseudonymization / Pseudonym Behaviour Printer")]
    public class PseudonymBehaviourPrinter : MonoBehaviour
    {
        [Tooltip("Pseudonym behaviour whose generated pseudonyms are logged.")]
        public PseudonymBehaviour pseudonymBehaviour;

        private void OnEnable()
        {
            // do nothing
        }

        private void Awake()
        {
            pseudonymBehaviour.OnCreatedPseudonym += pseudonym =>
            { 
                DebugLog.OmiLAXR.Print($"Generated pseudonym from '{pseudonymBehaviour.GetType()}': {pseudonym}");
            };
        }
    }
}