using UnityEngine;
using Doulingo.Core;
using Doulingo.Factory;
using Doulingo.Config;
using Doulingo.Logging;
using Doulingo.Chart;
using Doulingo.Input;
using Doulingo.Audio;

namespace Doulingo.Gameplay
{
    public class ServiceHub : MonoBehaviour
    {
        // Singleton instance
        public static ServiceHub Instance { get; private set; }

        // Global Services (initialized once on boot)
        public static IConfigProvider ConfigProvider { get; private set; }
        public static IEventLogger EventLogger { get; private set; }
        public static IAudioService AudioService { get; private set; }
        public static IInputReader InputReader { get; private set; }

        // GamePlay Services (rebuilt for each level)
        public static IJudgeService JudgeService { get; private set; }
        public static IScoringService ScoringService { get; private set; }
        public static IConductor Conductor { get; private set; }
        public static INoteTimeline NoteTimeline { get; private set; }
        public static INoteSpawner NoteSpawner { get; private set; }
        public static IStaffDrawer StaffDrawer { get; private set; }

        [Header("Global Service Factories (Boot-time)")]
        [SerializeField] private ConfigProviderFactory configProviderFactory;
        [SerializeField] private AudioServiceFactory audioServiceFactory;
        [SerializeField] private EventLoggerFactory eventLoggerFactory;

        [Header("GamePlay Service Factories (Level-specific)")]
        [SerializeField] private ConductorFactory conductorFactory;
        [SerializeField] private NoteTimelineFactory noteTimelineFactory;
        [SerializeField] private NoteSpawnerFactory noteSpawnerFactory;
        [SerializeField] private StaffDrawerFactory staffDrawerFactory;
        [SerializeField] private InputReaderFactory inputReaderFactory;

        private JudgeServiceFactory judgeServiceFactory = new();
        private ScoringServiceFactory scoringServiceFactory = new();


        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[ServiceHub] Singleton instance created");
            }
            else
            {
                Debug.LogWarning("[ServiceHub] Duplicate ServiceHub found, destroying this one");
                Destroy(gameObject);
            }

        }

        private void Start()
        {
            // Initialize Global Services once on boot
            InitializeGlobalServices();
        }

        /// <summary>
        /// Initialize Global Services once on boot (ConfigProvider, EventLogger, AudioService, InputReader)
        /// </summary>
        private void InitializeGlobalServices()
        {
            Debug.Log("[ServiceHub] Initializing Global Services...");

            // Validate global service factories
            if (configProviderFactory == null || audioServiceFactory == null ||
                inputReaderFactory == null || eventLoggerFactory == null)
            {
                Debug.LogError("[ServiceHub] One or more global service factories are not assigned!");
                return;
            }

            try
            {
                // Initialize ConfigProvider
                var configProvider = configProviderFactory.GetProvider(ConfigProviderType.RemoteConfigProvider);
                Debug.Log("[ServiceHub] ConfigProvider initialized");

                // Initialize EventLogger
                var eventLogger = eventLoggerFactory.GetEventLogger(EventLoggerType.EventLogger);
                Debug.Log("[ServiceHub] EventLogger initialized");

                // Initialize AudioService
                var audioService = audioServiceFactory.GetAudioService(AudioServiceType.UnityAudioService);
                Debug.Log("[ServiceHub] AudioService initialized");

                // Create Global Services instance
                ConfigProvider = configProvider;
                EventLogger = eventLogger;
                AudioService = audioService;

                Debug.Log("[ServiceHub] All Global Services initialized successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ServiceHub] Failed to initialize Global Services: {e.Message}");
            }
        }

        /// <summary>
        /// Initialize GamePlay Services for a specific level (JudgeService, ScoringService, Conductor, NoteTimeline, NoteSpawner, StaffDrawer)
        /// </summary>
        public void InitializeGamePlayServices(LevelConfig levelConfig)
        {
            if (levelConfig == null)
            {
                Debug.LogError("[ServiceHub] LevelConfig is null!");
                return;
            }

            Debug.Log($"[ServiceHub] Initializing GamePlay Services for level: {levelConfig.LevelName}");

            // Validate gameplay service factories
            if (judgeServiceFactory == null || scoringServiceFactory == null || conductorFactory == null ||
                noteTimelineFactory == null || noteSpawnerFactory == null || staffDrawerFactory == null)
            {
                Debug.LogError("[ServiceHub] One or more gameplay service factories are not assigned!");
                return;
            }

            try
            {
                // Get game config from global services
                var gameConfig = ConfigProvider?.GetGameConfig();
                if (gameConfig == null)
                {
                    Debug.LogError("[ServiceHub] Failed to get game config from ConfigProvider!");
                    return;
                }

                // Initialize JudgeService
                var judgeService = judgeServiceFactory.GetJudgeService(levelConfig.JudgeServiceType);
                if (judgeService != null)
                {
                    judgeService.SetJudgmentWindows(
                        gameConfig.hitWindow * 0.5f,
                        gameConfig.hitWindow * 0.75f,
                        gameConfig.hitWindow
                    );
                    Debug.Log($"[ServiceHub] JudgeService initialized with type: {levelConfig.JudgeServiceType}");
                }

                // Initialize ScoringService
                var scoringService = scoringServiceFactory.GetScoringService(levelConfig.ScoringServiceType);
                if (scoringService != null)
                {
                    Debug.Log($"[ServiceHub] ScoringService initialized with type: {levelConfig.ScoringServiceType}");
                }

                // Initialize Conductor
                var conductor = conductorFactory.GetConductor(levelConfig.ConductorType, levelConfig.InitialBPM);
                if (conductor != null)
                {
                    Debug.Log($"[ServiceHub] Conductor initialized with type: {levelConfig.ConductorType}");
                }

                // Initialize NoteTimeline
                var noteTimeline = noteTimelineFactory.GetNoteTimeline(levelConfig.NoteTimelineType);
                if (noteTimeline != null)
                {
                    // Load chart data if available
                    if (!string.IsNullOrEmpty(levelConfig.ChartData))
                    {
                        noteTimeline.LoadChart(levelConfig.ChartData);
                        Debug.Log($"[ServiceHub] NoteTimeline initialized with type: {levelConfig.NoteTimelineType}");
                    }
                    else
                    {
                        Debug.LogWarning($"[ServiceHub] No chart data in LevelConfig for {levelConfig.NoteTimelineType}");
                    }
                }

                // Initialize NoteSpawner
                var noteSpawner = noteSpawnerFactory.GetNoteSpawner(levelConfig.NoteSpawnerType);
                if (noteSpawner != null)
                {
                    Debug.Log($"[ServiceHub] NoteSpawner initialized with type: {levelConfig.NoteSpawnerType}");
                }

                // Initialize StaffDrawer
                var staffDrawer = staffDrawerFactory.GetStaffDrawer(levelConfig.StaffDrawerType);
                if (staffDrawer != null)
                {
                    // Initialize with default staff parameters
                    staffDrawer.Initialize(3f, -5f, 12f, 1f);
                    Debug.Log($"[ServiceHub] StaffDrawer initialized with type: {levelConfig.StaffDrawerType}");
                }

                var inputReader = inputReaderFactory.GetInputReader(levelConfig.InputReaderType);
                if (inputReader != null)
                {
                    inputReader.Initialize();
                }

                // Create GamePlay Services instance
                JudgeService = judgeService;
                ScoringService = scoringService;
                Conductor = conductor;
                NoteTimeline = noteTimeline;
                NoteSpawner = noteSpawner;
                StaffDrawer = staffDrawer;
                InputReader = inputReader;

                Debug.Log("[ServiceHub] All GamePlay Services initialized successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ServiceHub] Failed to initialize GamePlay Services: {e.Message}");
            }
        }

        /// <summary>
        /// Cleanup GamePlay Services when level ends
        /// </summary>
        public void CleanupGamePlayServices()
        {
            Debug.Log("[ServiceHub] Cleaning up GamePlay Services...");

            // Cleanup services in reverse order
            if (StaffDrawer != null)
            {
                StaffDrawer.ClearStaff();
            }

            if (NoteSpawner != null)
            {
                NoteSpawner.Clear();
            }

            if (NoteTimeline != null)
            {
                NoteTimeline.Clear();
            }

            if (Conductor != null)
            {
                Conductor.StopSong();
            }

            if (ScoringService != null)
            {
                ScoringService.ResetAll();
            }

            // Clear GamePlay Services
            JudgeService = null;
            ScoringService = null;
            Conductor = null;
            NoteTimeline = null;
            NoteSpawner = null;
            StaffDrawer = null;

            Debug.Log("[ServiceHub] All GamePlay Services cleaned up");
        }

        /// <summary>
        /// Check if GamePlay Services are ready
        /// </summary>
        public bool AreGamePlayServicesReady()
        {
            return JudgeService != null && ScoringService != null &&
                   Conductor != null && NoteTimeline != null && NoteSpawner != null &&
                   StaffDrawer != null;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ConfigProvider = null;
                EventLogger = null;
                AudioService = null;
                InputReader = null;
                JudgeService = null;
                ScoringService = null;
                Conductor = null;
                NoteTimeline = null;
                NoteSpawner = null;
                StaffDrawer = null;
                Debug.Log("[ServiceHub] Singleton instance destroyed");
            }
        }
    }
}
