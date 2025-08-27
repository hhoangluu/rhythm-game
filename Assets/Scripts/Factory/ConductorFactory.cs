using UnityEngine;
using Doulingo.Core;
using Doulingo.Audio;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating conductors based on type using prefabs
    /// </summary>
    public class ConductorFactory : MonoBehaviour
    {
        [Header("Conductor Prefabs")]
        [SerializeField] private Conductor conductorPrefab;
        
        /// <summary>
        /// Gets a conductor instance based on the specified type
        /// </summary>
        /// <param name="type">Type of conductor to create</param>
        /// <param name="initialBPM">Initial BPM for the conductor</param>
        /// <returns>Conductor instance</returns>
        public IConductor GetConductor(ConductorType type, float initialBPM = 120f)
        {
            IConductor conductor = type switch
            {
                ConductorType.Conductor => Instantiate(conductorPrefab),
                _ => Instantiate(conductorPrefab)
            };
            
            return conductor;
        }
    }
}
