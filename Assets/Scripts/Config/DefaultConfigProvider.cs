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
                hitWindow = 0.15f,
                minBPM = 80f,
                maxBPM = 180f,
                cooldown = 0.8f,
                streakForIncrease = 3,
                missesForDecrease = 2,
                bpmChangeAmount = 8f
            };
        }
    }
}
