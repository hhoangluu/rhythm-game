namespace Doulingo.Core
{
    /// <summary>
    /// Context struct containing all core services that features need to access
    /// </summary>
    public readonly struct GameContext
    {
        public readonly IAudioService AudioService { get; }
        public readonly IConductor Conductor { get; }
        public readonly INoteTimeline NoteTimeline { get; }
        public readonly IConfigProvider ConfigProvider { get; }
        public readonly IEventLogger EventLogger { get; }
        public readonly GameConfig GameConfig { get; }
        public readonly IScoringService ScoringService { get; }

        public GameContext(
            IAudioService audioService,
            IConductor conductor,
            INoteTimeline noteTimeline,
            IConfigProvider configProvider,
            IEventLogger eventLogger,
            GameConfig gameConfig,
            IScoringService scoringService)
        {
            AudioService = audioService;
            Conductor = conductor;
            NoteTimeline = noteTimeline;
            ConfigProvider = configProvider;
            EventLogger = eventLogger;
            GameConfig = gameConfig;
            ScoringService = scoringService;
        }
    }
}
