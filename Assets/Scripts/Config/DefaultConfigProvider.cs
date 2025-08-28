using UnityEngine;
using Doulingo.Core;

namespace Doulingo.Config
{
    /// <summary>
    /// Default config provider with fallback values
    /// </summary>
    public class DefaultConfigProvider : IConfigProvider
    {
        public GameConfig GetGameConfig()
        {
            return new GameConfig
            {
                initialBPM = 100f,
                hitWindow = 0.15f
            };
        }
    }
}
