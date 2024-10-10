// Author: David Baberowski <david.baberowski@tu-dresden.de>
// Adapted for Ecosystem by: Sergej Görzen <goerzen@cs.rwth-aachen.de>

using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace OmiLAXR.Pseudonymization
{
    /// <summary>
    /// Is generating a random Pseudonym with a chartset of "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("OmiLAXR / Pseudonymization / Random Pseudonym Generator")]
    public class RandomPseudonymGenerator : PseudonymBehaviour
    {
        public static RandomPseudonymGenerator Instance { get; private set; }
        
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
        
        private static readonly System.Random Random = new System.Random();
        
        protected override Task<string> GenerateNewPseudonym()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return Task.FromResult(new string(
                Enumerable.Repeat(chars, 6).Select(s => s[Random.Next(s.Length)]).ToArray()));
        }
    }
}