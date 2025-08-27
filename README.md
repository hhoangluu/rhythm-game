# Doulingo Music - Unity Mini-Game

A Unity mini-game project similar to Duolingo Music where players press the correct piano keys at the right time in a rhythm-based gameplay experience.

## Features

- **Rhythm Gameplay**: Press piano keys in sequence as notes fall down the screen
- **Piano Keyboard Interface**: 8-key piano layout (C, D, E, F, G, A, B, C)
- **Progressive Difficulty**: BPM increases by +10 after 4 consecutive hits, decreases by -10 after 3 consecutive misses
- **Configurable Settings**: Hit window and BPM can be changed via RemoteConfig.json without rebuilding
- **JSON Logging**: Console output for note_hit, note_miss, and song_end events with piano key information
- **SOLID Architecture**: Clean, maintainable code following SOLID principles
- **No MonoBehaviour Dependencies**: Core services are pure C# classes for better testability

## Gameplay

The game follows a **falling note rhythm game** pattern similar to Guitar Hero or Rock Band:

1. **Notes fall down** from the top of the screen in lanes
2. **Piano keys** are displayed at the bottom (A, S, D, F, G, H, J, K keys)
3. **Player presses the correct piano key** when the note reaches the target line
4. **Timing matters** - hit the key at the right moment for better scores
5. **Progressive difficulty** - BPM increases/decreases based on performance

### Controls
- **A Key** → C note (Index 0)
- **S Key** → D note (Index 1)  
- **D Key** → E note (Index 2)
- **F Key** → F note (Index 3)
- **G Key** → G note (Index 4)
- **H Key** → A note (Index 5)
- **J Key** → B note (Index 6)
- **K Key** → C note (Index 7)

## Architecture

The project follows a clean architecture pattern with clear separation of concerns:

### Core Interfaces (`Assets/Scripts/Core/`)
- `IConfigProvider` - Game configuration management
- `IConductor` - Song timing and BPM control
- `INoteTimeline` - Note data and queries
- `IInputReader` - Piano key input handling
- `IJudgeService` - Note judgment and hit windows
- `IScoringService` - Score, combo, and streak tracking
- `IDifficultyScaler` - BPM scaling based on performance
- `IEventLogger` - JSON event logging

### Factory System (`Assets/Scripts/Factory/`)
- `ConfigProviderType` - Enum for config provider types
- `ConfigProviderFactory` - Factory for creating config providers
- `ConductorType` - Enum for conductor types
- `ConductorFactory` - Factory for creating conductors
- `NoteTimelineType` - Enum for note timeline types
- `NoteTimelineFactory` - Factory for creating note timelines
- `InputReaderType` - Enum for input reader types
- `InputReaderFactory` - Factory for creating input readers
- `EventLoggerType` - Enum for event logger types
- `EventLoggerFactory` - Factory for creating event loggers
- `NoteSpawnerType` - Enum for note spawner types
- `NoteSpawnerFactory` - Factory for creating note spawners
- `JudgeServiceType` - Enum for judge service types
- `JudgeServiceFactory` - Factory for creating judge services
- `ScoringServiceType` - Enum for scoring service types
- `ScoringServiceFactory` - Factory for creating scoring services
- `DifficultyScalerType` - Enum for difficulty scaler types
- `DifficultyScalerFactory` - Factory for creating difficulty scalers

### Service Hub (`Assets/Scripts/Gameplay/`)
- `ServiceHub` - Central coordinator that uses ALL factories to build all game systems
- `GameSystems` - Container class for all built game systems

### Implementation Classes
- `RemoteConfigProvider` - Loads configuration from JSON (MonoBehaviour)
- `Conductor` - Manages song timing and BPM changes (MonoBehaviour)
- `NoteTimeline` - Handles note data and spawning (MonoBehaviour)
- `InputReader` - Processes piano key input (MonoBehaviour)
- `JudgeService` - Judges note hits based on timing (Pure C# class)
- `ScoringService` - Tracks score, combo, and streaks (Pure C# class)
- `DifficultyScaler` - Applies BPM scaling rules (Pure C# class)
- `EventLogger` - Logs game events as JSON (MonoBehaviour)
- `GameManager` - Orchestrates all game systems using ServiceHub (MonoBehaviour)
- `NoteSpawner` - Manages visual note objects (MonoBehaviour)
- `NoteObject` - Individual note behavior and visuals (MonoBehaviour)
- `GameUI` - User interface management (MonoBehaviour)

## Key Architectural Benefits

- **Pure C# Services**: Core game logic (JudgeService, ScoringService, DifficultyScaler) are pure C# classes without MonoBehaviour dependencies
- **Better Testability**: Pure C# services can be easily unit tested
- **Cleaner Dependencies**: Services don't inherit from MonoBehaviour, reducing coupling
- **Inspector Integration**: ServiceContainer provides Unity Inspector integration for pure C# services
- **Event-Driven**: All systems communicate through events, maintaining loose coupling
- **Piano-Focused**: Input system designed specifically for piano key rhythm gameplay

## Configuration

Game settings are stored in `Assets/StreamingAssets/RemoteConfig.json`:

```json
{
  "initialBPM": 120.0,
  "hitWindow": 0.2,
  "minBPM": 60.0,
  "maxBPM": 200.0,
  "cooldown": 1.0,
  "streakForIncrease": 4,
  "missesForDecrease": 3,
  "bpmChangeAmount": 10.0
}
```

### Settings Explained
- `initialBPM`: Starting tempo of the song
- `hitWindow`: Time window (in seconds) for hitting notes
- `minBPM`/`maxBPM`: BPM range limits
- `cooldown`: Time between difficulty changes
- `streakForIncrease`: Hit streak needed to increase difficulty
- `missesForDecrease`: Miss streak that triggers difficulty decrease
- `bpmChangeAmount`: How much BPM changes on difficulty shifts

## Chart Format

Charts are simple text files that define note timing and piano keys. The format is:

```
beat,type,duration,pianoKeyIndex
```

### Format Details
- **beat**: Timing in beats (e.g., 0.0, 1.0, 2.0)
- **type**: Note type (Tap, Hold, or Slide)
- **duration**: How long to hold (for Hold notes, 0.0 for Tap notes)
- **pianoKeyIndex**: Which piano key (0-7) this note corresponds to

### Example Chart
```
# Test Chart for Doulingo Music Piano Game
# Format: beat,type,duration,pianoKeyIndex
0.0,Tap,0.0,0
1.0,Tap,0.0,1
2.0,Tap,0.0,2
3.0,Tap,0.0,3
```

### Piano Key Mapping
- **Index 0**: C note (A key)
- **Index 1**: D note (S key)
- **Index 2**: E note (D key)
- **Index 3**: F note (F key)
- **Index 4**: G note (G key)
- **Index 5**: A note (H key)
- **Index 6**: B note (J key)
- **Index 7**: C note (K key)

**Note**: Lane positions are calculated automatically based on piano key index - no need to specify x,y,z coordinates!

## Event Logging

The game logs all important events as JSON to the console with piano key information:

### Note Hit Event
```json
{
  "event_type": "note_hit",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "note_beat": 1.0,
  "note_type": "Tap",
  "hit_result": "Perfect",
  "accuracy": 0.95,
  "piano_key_index": 2,
  "piano_key_name": "E",
  "lane_number": 2
}
```

### Note Miss Event
```json
{
  "event_type": "note_miss",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "note_beat": 2.0,
  "note_type": "Tap",
  "piano_key_index": 3,
  "piano_key_name": "F",
  "lane_number": 3
}
```

### Song End Event
```json
{
  "event_type": "song_end",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "final_score": 1500,
  "total_hits": 15,
  "total_misses": 2,
  "accuracy_percentage": 88.2
}
```

## Scene Setup

1. **ServiceHub-Based System Creation**: Use ServiceHub to automatically create and manage all game systems
2. **Single Point of Configuration**: GameManager only needs to reference ServiceHub
3. **Automatic System Building**: ServiceHub coordinates all factories and services
4. **Configuration**: Ensure RemoteConfig.json is properly set up
5. **Prefab Management**: Create prefabs for each system type with required components

### Required GameObjects:
- **ServiceHub** (with ServiceHub script) - Central coordinator for all systems
- **ConfigProviderFactory** (with ConfigProviderFactory script) - Creates config providers
- **ConductorFactory** (with ConductorFactory script) - Creates conductors
- **NoteTimelineFactory** (with NoteTimelineFactory script) - Creates note timelines
- **InputReaderFactory** (with InputReaderFactory script) - Creates input readers
- **JudgeServiceFactory** (with JudgeServiceFactory script) - Creates judge services
- **ScoringServiceFactory** (with ScoringServiceFactory script) - Creates scoring services
- **DifficultyScalerFactory** (with DifficultyScalerFactory script) - Creates difficulty scalers
- **EventLoggerFactory** (with EventLoggerFactory script) - Creates event loggers
- **NoteSpawnerFactory** (with NoteSpawnerFactory script) - Creates note spawners
- **GameManager** (with GameManager script) - Orchestrates the game using ServiceHub

### ServiceHub Setup:
1. **Assign System Factories**: ServiceHub needs references to all system factory GameObjects
2. **Assign Service Factories**: ServiceHub needs references to all service factory GameObjects
3. **Assign Other System Factories**: ServiceHub needs references to EventLogger and NoteSpawner factories
4. **System Building**: ServiceHub automatically builds all systems when `BuildFromConfig()` is called

### GameManager Setup:
1. **Single Reference**: Only assign ServiceHub reference
2. **Automatic System Access**: GameManager gets all systems via `serviceHub.BuildFromConfig()`
3. **No Factory Knowledge**: GameManager doesn't need to know about factories or system types

### Benefits:
- **Single Point of Configuration**: GameManager only references ServiceHub
- **Automatic System Management**: ServiceHub handles all factory coordination
- **Clean Architecture**: Follows Facade Pattern and Dependency Inversion Principle
- **Easy to Extend**: Can add new system types by extending factories and ServiceHub
- **Simplified GameManager**: No more complex factory management code

## How to Use

1. **Setup**: Open the project in Unity 2022.3 LTS or later
2. **Configure**: Modify `RemoteConfig.json` to adjust game settings
3. **Assign References**: Drag references in the Inspector (no FindObjectOfType needed!)
4. **Configure Piano Keys**: Set up piano key mappings in InputReader
5. **Play**: Press Play in Unity or build and run the game
6. **Controls**: 
   - Piano Keys: A, S, D, F, G, H, J, K for different notes
   - Mouse: Click for testing/debugging
7. **Objective**: Press the correct piano key when notes reach the target line
8. **Scoring**: Build combos for higher scores, maintain accuracy for difficulty increases

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Interfaces and core types
│   ├── Factory/        # Factory system for creating game objects
│   ├── Config/         # Configuration management
│   ├── Audio/          # Conductor and timing
│   ├── Chart/          # Note timeline and parsing
│   ├── Input/          # Piano key input handling
│   ├── Gameplay/       # Core game systems
│   ├── UI/             # User interface
│   ├── Logging/        # Event logging
│   └── Setup/          # Scene setup guides
├── Scenes/             # Unity scenes
├── StreamingAssets/    # Configuration and chart files
└── ...
```

## Dependencies

- Unity 2022.3 LTS or later
- TextMeshPro (included with Unity)
- No external packages required

## Development

The project is designed to be easily extensible:

- Add new note types by extending the `NoteType` enum
- Implement new judgment systems by extending `IJudgeService`
- Add new scoring mechanics through `IScoringService`
- Create custom charts by following the chart format specification
- Pure C# services can be easily unit tested
- No MonoBehaviour dependencies in core game logic
- Piano key system can be extended for more keys or different instruments

## License

This project is provided as-is for educational and development purposes.
