using UnityEngine;

namespace Doulingo.Core
{
    public interface IConfigProvider
    {
        GameConfig GetGameConfig();
    }

    [System.Serializable]
    public class GameConfig
    {
        public float initialBPM = 120f;
        public float hitWindow = 0.2f;
        public float minBPM = 60f;
        public float maxBPM = 200f;
    }
}
