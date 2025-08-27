using UnityEngine;
using Doulingo.Core;
using Doulingo.Config;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating config providers based on type using prefabs
    /// </summary>
    public class ConfigProviderFactory : MonoBehaviour
    {
        [Header("Config Provider Prefabs")]
        [SerializeField] private RemoteConfigProvider remoteConfigProviderPrefab;
        
        /// <summary>
        /// Gets a config provider instance based on the specified type
        /// </summary>
        /// <param name="type">Type of config provider to create</param>
        /// <returns>Config provider instance</returns>
        public IConfigProvider GetProvider(ConfigProviderType type)
        {
            return type switch
            {
                ConfigProviderType.RemoteConfigProvider => Instantiate(remoteConfigProviderPrefab),
                _ => Instantiate(remoteConfigProviderPrefab)
            };
        }
    }
}
