using UnityEngine;
using Doulingo.Core;
using Doulingo.Audio;
using Doulingo.Chart;
using Doulingo.Input;
using Doulingo.Gameplay;
using Doulingo.Logging;
using Doulingo.Config;
using Doulingo.Factory;
using Doulingo.Features;
using System.Collections.Generic;
using System;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Main game orchestrator that manages all game systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }

        [Header("Service Hub")]
        [SerializeField] private ServiceHub serviceHub;

        // Game configuration
        private GameConfig gameConfig;
        private LevelConfig levelConfig;
        public LevelConfig LevelConfig => levelConfig;


        // System references (obtained from ServiceHub)
        private IConfigProvider configProvider;
        private IConductor conductor;
        private INoteTimeline noteTimeline;
        private IInputReader inputReader;
        private IJudgeService judgeService;
        private IScoringService scoringService;
        private IAudioService audioService;
        private IEventLogger eventLogger;
        private INoteSpawner noteSpawner;

        private IStaffDrawer staffDrawer;
        [SerializeField]
        private SfxPlayer sfxPlayer;

        [SerializeField]
        private HitLineController hitLineController;

        // Feature system
        private FeatureManager featureManager;
        private GameContext gameContext;

        // Game state
        private bool isGameRunning = false;
        private float gameStartTime;

        // Events
        public static event Action OnInitializeSystemsComplete;
        public static event Action OnGameStart;
        public static event Action OnSongEnd; // Event for when song ends
        public static event Action<NoteData, HitResult> OnNoteJudged; // Event for note judgment results
        public static event Action<int, float> OnKeyUp;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[GameManager] Singleton instance created");
            }
            else
            {
                Debug.LogWarning("[GameManager] Duplicate GameManager found, destroying this instance");
                Destroy(gameObject);
            }
        }

        public void SetLevelConfig(LevelConfig config)
        {
            levelConfig = config;
        }

        public void StartGame()
        {
            isGameRunning = true;
            gameStartTime = Time.time;


            // Set song end beat (last note + 2 beats delay)
            if (noteTimeline != null)
            {
                float totalNotes = noteTimeline.GetLastNoteBeat();
                conductor.Initialize(levelConfig.InitialBPM, levelConfig.SongStartDelay, totalNotes, audioService);
            }


            // Start audio (aligned with Conductor)
            if (audioService != null)
            {
                audioService.Play(AudioSettings.dspTime + levelConfig.SongStartDelay);

                // Optional click track
                audioService.EnableClick(levelConfig.EnableClickTrack);
                audioService.SetClickSubdivision(1); // 1 click per beat
                audioService.SetClickVolume(0.3f);  // comfortable volume
            }

            // Start conductor (this will trigger visual updates)
            conductor.StartSong();

            // Initialize spawner and spawn beat lines
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(noteTimeline, conductor);
            }

            if (staffDrawer != null)
            {
                staffDrawer.DrawStaff();
            }

            hitLineController.Init();

            featureManager?.OnGameStart();

            Debug.Log($"[GameManager] Game started with BPM: {levelConfig.InitialBPM}, delay: {levelConfig.SongStartDelay}s");
            eventLogger.LogGameStart(levelConfig.InitialBPM);
            OnGameStart?.Invoke();
        }

        private void Update()
        {
            if (isGameRunning)
            {
                UpdateGame();
            }
        }

        public void InitializeGame()
        {
            if (levelConfig == null)
            {
                Debug.LogError("LevelConfig not set!");
                return;
            }

            if (serviceHub == null)
            {
                Debug.LogError("ServiceHub not assigned!");
                return;
            }



            // Extract systems

            audioService = ServiceHub.AudioService;
            eventLogger = ServiceHub.EventLogger;


            InitializeSystems();
            SetupEventHandlers();

            // Validate all systems
            if (conductor == null || noteTimeline == null || inputReader == null ||
                judgeService == null || scoringService == null || audioService == null ||
                eventLogger == null || noteSpawner == null || sfxPlayer == null) // Add sfxPlayer to validation
            {
                Debug.LogError("One or more game systems failed to initialize!");
                return;
            }
            // Fire event when all systems are initialized
            OnInitializeSystemsComplete?.Invoke();

        }

        private void InitializeSystems()
        {
            serviceHub.InitializeGamePlayServices(levelConfig);
            conductor = ServiceHub.Conductor;
            noteTimeline = ServiceHub.NoteTimeline;
            inputReader = ServiceHub.InputReader;
            judgeService = ServiceHub.JudgeService;
            scoringService = ServiceHub.ScoringService;
            noteSpawner = ServiceHub.NoteSpawner;
            configProvider = ServiceHub.ConfigProvider;
            staffDrawer = ServiceHub.StaffDrawer;

            gameConfig = configProvider.GetGameConfig();

            // // Setup audio service
            SetupAudioService();
            // // Set up feature system
            SetupFeatureSystem();
            noteSpawner.Initialize(noteTimeline, conductor);

            // Load chart

            Debug.Log("All game systems initialized successfully!");
        }


        private void SetupAudioService()
        {
            if (audioService != null && levelConfig?.SongDescriptor != null)
            {
                // Load the song from level config
                audioService.LoadSong(levelConfig.SongDescriptor);

                Debug.Log($"[GameManager] Audio service loaded song: {levelConfig.SongDescriptor.songId} at {levelConfig.SongDescriptor.bpmBase} BPM");
            }
            else
            {
                Debug.LogWarning("[GameManager] Audio service or song descriptor not available, skipping audio setup");
            }
        }

        private void SetupFeatureSystem()
        {
            // Create feature manager
            featureManager = new FeatureManager();

            // Create game context
            gameContext = new GameContext(
                audioService,
                conductor,
                noteTimeline,
                configProvider,
                eventLogger,
                gameConfig,
                scoringService
            );

            // Add features based on game mode configuration
            if (levelConfig != null)
            {
                if (levelConfig.EnableVocalGate)
                {
                    var vocalGateFeature = new VocalGateFeature();
                    vocalGateFeature.Configure(
                        levelConfig.VocalMissGate,
                        levelConfig.VocalHitGate,
                        levelConfig.VocalFadeMs
                    );
                    featureManager.Add(vocalGateFeature);
                    Debug.Log($"[GameManager] Added VocalGateFeature with missGate={levelConfig.VocalMissGate}, hitGate={levelConfig.VocalHitGate}");
                }

                if (levelConfig.EnableDifficultyScaling)
                {
                    var difficultyFeature = new DifficultyScalerFeature();
                    difficultyFeature.Configure(
                        levelConfig.DifficultyUpGate,
                        levelConfig.DifficultyDownGate,
                        levelConfig.DifficultyBpmStep,
                        levelConfig.DifficultyCooldown
                    );

                    // Wire up difficulty change events
                    difficultyFeature.OnDifficultyChanged += ApplyBPM;

                    featureManager.Add(difficultyFeature);
                    Debug.Log($"[GameManager] Added DifficultyScalerFeature with upGate={levelConfig.DifficultyUpGate}, downGate={levelConfig.DifficultyDownGate}");
                }
            }
            else
            {
                // Use default features if no config is provided
                var vocalGateFeature = new VocalGateFeature();
                var difficultyFeature = new DifficultyScalerFeature();

                featureManager.Add(vocalGateFeature);
                featureManager.Add(difficultyFeature);
                Debug.Log("[GameManager] Added default features (no game mode config)");
            }

            // Initialize all features
            featureManager.InitializeAll(gameContext);
            Debug.Log($"[GameManager] Feature system initialized with {featureManager.FeatureCount} features");
        }

        private void SetupEventHandlers()
        {
            // Conductor events
            conductor.OnBPMChanged += HandleBPMChanged;
            conductor.OnSongEnded += HandleSongEnd;

            // Input events - simplified to just key down/up
            inputReader.OnKeyDown += HandleKeyDown;
            inputReader.OnKeyUp += HandleKeyUp;

            // Scoring events
            scoringService.OnHitStreakChanged += HandleHitStreakChanged;
            scoringService.OnMissStreakChanged += HandleMissStreakChanged;
        }

        private void UpdateGame()
        {
            // Update features
            featureManager?.Tick(Time.deltaTime);

            // Get current song position
            float currentBeat = conductor.SongPositionInBeats;
            // Check for notes that need to be spawned
            var notesToSpawn = noteTimeline.GetNotesInRange(currentBeat, currentBeat + 15f);
            foreach (var note in notesToSpawn)
            {
                if (!noteSpawner.IsNoteSpawned(note))
                {
                    noteSpawner.SpawnNote(note, currentBeat);
                }
            }

            // Check for missed notes
            var missedNotes = noteTimeline.GetNotesInRange(currentBeat - 1f, currentBeat - gameConfig.hitWindow);
            foreach (var note in missedNotes)
            {
                if (noteSpawner.IsNoteSpawned(note) && !noteSpawner.IsNoteProcessed(note))
                {
                    HandleMissedNote(note);
                }
            }
        }

        private void HandleKeyDown(int keyIndex)
        {
            // Play sound for key press directly from SfxPlayer
            sfxPlayer.PlayKey(keyIndex);

            // Handle note input logic
            float currentBeat = conductor.SongPositionInBeats;
            var nearbyNotes = noteTimeline.GetNotesInRange(currentBeat - gameConfig.hitWindow, currentBeat + gameConfig.hitWindow);

            foreach (var note in nearbyNotes)
            {
                if (note.pianoKeyIndex == keyIndex && !noteSpawner.IsNoteProcessed(note))
                {
                    // Let JudgeService calculate the hit accuracy
                    var judgment = judgeService.JudgeNote(note, currentBeat);

                    if (judgment != HitResult.Miss)
                    {
                        // Get the actual hit accuracy from JudgeService
                        float hitAccuracy = judgeService.GetHitAccuracy(note, currentBeat);
                        HandleHitNote(note, judgment, hitAccuracy);
                        hitLineController.ShowKey((PianoNote)keyIndex, note);
                        return;
                    }
                }
            }
            hitLineController.ShowKey((PianoNote)keyIndex, null);

        }



        private void HandleKeyUp(int keyIndex, float holdTime)
        {
            // Stop sound for key release directly from SfxPlayer
            sfxPlayer.StopKey(keyIndex);
            hitLineController.ReleaseKey((PianoNote)keyIndex);
            OnKeyUp?.Invoke(keyIndex, holdTime);
        }

        private void HandleHitNote(NoteData note, HitResult judgment, float accuracy)
        {
            note.isHited = true;
            noteSpawner.ProcessNote(note);
            scoringService.ProcessHit(judgment);

            // Notify features about the hit
            featureManager?.OnHit(note, judgment, accuracy);

            // Fire note judged event for UI
            OnNoteJudged?.Invoke(note, judgment);

            eventLogger.LogNoteHit(note, judgment, accuracy);
            // Debug.Log($"Note hit! Beat: {note.beat}, Key: {note.pianoKeyIndex}, Judgment: {judgment}");
        }

        private void HandleMissedNote(NoteData note)
        {
            note.isHited = false;
            noteSpawner.ProcessNote(note);
            scoringService.ProcessMiss();

            // Notify features about the miss
            featureManager?.OnMiss(note);

            // Fire note judged event for UI (MISS result)
            OnNoteJudged?.Invoke(note, HitResult.Miss);

            eventLogger.LogNoteMiss(note);
            Debug.Log($"Note missed! Beat: {note.beat}, Key: {note.pianoKeyIndex}");
        }

        private void HandleBPMChanged(float newBPM)
        {
            Debug.Log($"BPM changed to: {newBPM}");
            //eventLogger.LogBPMChange(newBPM);
        }

        private void HandleSongEnd()
        {
            isGameRunning = false;
            float gameDuration = Time.time - gameStartTime;

            // Notify features that game has ended
            featureManager?.OnGameEnd();

            // Reset all services
            ResetAllServices();

            eventLogger.LogSongEnd(scoringService.TotalScore, scoringService.TotalHits, scoringService.TotalMisses);
            Debug.Log($"Song ended! Final Score: {scoringService.TotalScore}, Hits: {scoringService.TotalHits}, Misses: {scoringService.TotalMisses}");
            OnSongEnd?.Invoke(); // Fire the new event
        }

        private void ResetAllServices()
        {
            // Stop audio
            audioService?.Stop();
            audioService.EnableClick(false);

            // Stop conductor
            // conductor?.StopSong();

            // Reset scoring
            scoringService?.ResetAll();

            // Clear all notes and beat lines
            noteSpawner?.Clear();

            staffDrawer?.ClearStaff();
            // Reset input reader
            inputReader?.Cleanup();

            noteTimeline.Clear();
            hitLineController.Stop();

            // Reset features
            featureManager?.OnGameEnd();

            Debug.Log("[GameManager] All services reset");
        }

        private void HandleHitStreakChanged(int newStreak)
        {
            Debug.Log($"Hit streak: {newStreak}");

            // Notify features about hit streak change
            featureManager?.OnHitStreakChanged(newStreak);
        }

        private void HandleMissStreakChanged(int newStreak)
        {
            Debug.Log($"Miss streak: {newStreak}");

            // Notify features about miss streak change
            featureManager?.OnMissStreakChanged(newStreak);
        }

        private void OnDestroy()
        {
            // Clean up event handlers
            if (conductor != null)
            {
                conductor.OnBPMChanged -= HandleBPMChanged;
                conductor.OnSongEnded -= HandleSongEnd;
            }

            if (inputReader != null)
            {
                inputReader.OnKeyDown -= HandleKeyDown;
                inputReader.OnKeyUp -= HandleKeyUp;
            }

            if (scoringService != null)
            {
                scoringService.OnHitStreakChanged -= HandleHitStreakChanged;
                scoringService.OnMissStreakChanged -= HandleMissStreakChanged;
            }
        }

        /// <summary>
        /// Changes BPM and synchronizes both Conductor and AudioService
        /// </summary>
        private void ApplyBPM(float newBpm)
        {
            if (conductor == null || audioService == null || levelConfig?.SongDescriptor == null)
                return;

            float baseBpm = levelConfig.SongDescriptor.bpmBase;
            float ratio = newBpm / baseBpm;

            // Update visual/beat math
            conductor.ChangeBPM(newBpm);

            // Update music tempo (via pitch) and click timing
            audioService.SetPitch(ratio);

            Debug.Log($"[GameManager] BPM changed to {newBpm}, pitch ratio: {ratio:F2}");
        }
    }
}
