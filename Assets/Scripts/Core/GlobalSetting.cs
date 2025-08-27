using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Global settings that are automatically loaded on boot and accessible everywhere
    /// </summary>
    [CreateAssetMenu(fileName = "GlobalSetting", menuName = "Doulingo/Global Setting")]
    public class GlobalSetting : ScriptableObject
    {
        private static GlobalSetting instance;
        // Static instance for global access
        public static GlobalSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GlobalSetting>("GlobalSetting");
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        [Header("Staff Map Settings")]
        [SerializeField] private float unitsPerBeatBase = 3f;
        [SerializeField] private float hitLineX0 = 0f;
        [SerializeField] private float cleanupBeats = 2.0f;
        [SerializeField] private float verticalSpacing = 0.8f;
        [SerializeField] private float baseYPosition = 0.0f;

        // Static properties for global access
        public static float UnitsPerBeatBase => Instance?.unitsPerBeatBase ?? 3f;
        public static float HitLineX0 => Instance?.hitLineX0 ?? 0f;
        public static float CleanupBeats => Instance?.cleanupBeats ?? 2.0f;
        public static float VerticalSpacing => Instance?.verticalSpacing ?? 0.8f;
        public static float BaseYPosition => Instance?.baseYPosition ?? 0.0f;


        /// <summary>
        /// Force load GlobalSetting before everything else
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ForceLoadGlobalSetting()
        {
            // Load GlobalSetting from Resources folder
            var globalSetting = Resources.Load<GlobalSetting>("GlobalSetting");
            if (globalSetting != null)
            {
                instance = globalSetting;
                Debug.Log("[GlobalSetting] Force loaded before scene load");
            }
            else
            {
                Debug.LogWarning("[GlobalSetting] Could not find GlobalSetting in Resources folder!");
            }
        }
    }
}
