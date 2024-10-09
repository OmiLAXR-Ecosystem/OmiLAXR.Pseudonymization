using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    [AddComponentMenu("OmiLAXR / Pseudonymization / Pseudonym Behaviour Printer")]
    public class PseudonymBehaviourPrinter : MonoBehaviour
    {
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