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
                hitWindow = 0.2f,
                minBPM = 60f,
                maxBPM = 200f,
                cooldown = 1f,
                streakForIncrease = 4,
                missesForDecrease = 3,
                bpmChangeAmount = 10f
            };
        }
    }
}
