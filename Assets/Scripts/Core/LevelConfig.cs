using Doulingo.Factory;
using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Configuration for different levels and their enabled features
    /// </summary>
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Doulingo/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level")]
        [SerializeField] private string levelName = "Default";
        [SerializeField] private string description = "Default level";
        [SerializeField] private SongDescriptor songDescriptor;
        [SerializeField] private TextAsset chartAsset; // Chart asset for NoteTimeline

        [Header("Audio Settings")]
        [SerializeField] private float initialBPM = 120f;
        [SerializeField] private float songStartDelay = 2.0f;
        [SerializeField] private bool enableClickTrack = true;

        [Header("Service Types")]
        [SerializeField] private JudgeServiceType judgeServiceType = JudgeServiceType.JudgeService;
        [SerializeField] private ScoringServiceType scoringServiceType = ScoringServiceType.ScoringService;
        [SerializeField] private ConductorType conductorType = ConductorType.Conductor;
        [SerializeField] private NoteTimelineType noteTimelineType = NoteTimelineType.NoteTimeline;
        [SerializeField] private NoteSpawnerType noteSpawnerType = NoteSpawnerType.NoteSpawner;
        [SerializeField] private StaffDrawerType staffDrawerType = StaffDrawerType.BeatLineStaffDrawer;
        [SerializeField] private InputReaderType inputReaderType = InputReaderType.InputReader;

        [Header("Feature Toggles")]
        [SerializeField] private bool enableVocalGate = true;
        [SerializeField] private bool enableDifficultyScaling = true;

        [Header("Vocal Gate Settings")]
        [SerializeField] private int vocalMissGate = 3;
        [SerializeField] private int vocalHitGate = 3;
        [SerializeField] private float vocalFadeMs = 500f;

        [Header("Difficulty Scaling Settings")]
        [SerializeField] private int difficultyUpGate = 4;
        [SerializeField] private int difficultyDownGate = 3;
        [SerializeField] private float difficultyBpmStep = 10f;
        [SerializeField] private float difficultyCooldown = 2f;

        // Properties
        public string LevelName => levelName;
        public string Description => description;
        public SongDescriptor SongDescriptor => songDescriptor;
        public string ChartData => chartAsset.text;
        
        // Audio Properties
        public float InitialBPM => initialBPM;
        public float SongStartDelay => songStartDelay;
        public bool EnableClickTrack => enableClickTrack;
        
        // Service Type Properties
        public JudgeServiceType JudgeServiceType => judgeServiceType;
        public ScoringServiceType ScoringServiceType => scoringServiceType;
        public ConductorType ConductorType => conductorType;
        public NoteTimelineType NoteTimelineType => noteTimelineType;
        public NoteSpawnerType NoteSpawnerType => noteSpawnerType;
        public StaffDrawerType StaffDrawerType => staffDrawerType;
        public InputReaderType InputReaderType => inputReaderType;
        
        // Feature Properties
        public bool EnableVocalGate => enableVocalGate;
        public bool EnableDifficultyScaling => enableDifficultyScaling;

        // Vocal Gate Settings
        public int VocalMissGate => vocalMissGate;
        public int VocalHitGate => vocalHitGate;
        public float VocalFadeMs => vocalFadeMs;

        // Difficulty Scaling Settings
        public int DifficultyUpGate => difficultyUpGate;
        public int DifficultyDownGate => difficultyDownGate;
        public float DifficultyBpmStep => difficultyBpmStep;
        public float DifficultyCooldown => difficultyCooldown;
    }
}
