using UnityEngine;
using Doulingo.Core;

namespace Doulingo.Config
{
    /// <summary>
    /// Local config provider that uses hardcoded values
    /// </summary>
    public class LocalConfigProvider : IConfigProvider
    {
        public GameConfig GetGameConfig()
        {
            return new GameConfig
            {
                initialBPM = 120f,
                hitWindow = 0.2f
            };
        }
    }
}
