# Doulingo Music - Unity Mini-Game

A Unity mini-game project similar to Duolingo Music where players press the correct piano keys at the right time in a rhythm-based gameplay experience.

## 🎥 Video Demo

📺 **[Watch Game Demo](https://drive.google.com/file/d/1eIVJfKV39FCf0mGYqIg-iEPqVVBVpM0s/view?usp=sharing)**

See the game in action with piano key gameplay, difficulty scaling, and vocal gate features!

## How to Start 

1. **Start MainScene** - Open the MainScene in Unity
2. **Choose Gameplay Mode** - Select between:
   - **Mode 1**: Rhythm Tap (60 BPM, difficulty scaling enabled)
   - **Mode 2**: Performance Mode (120 BPM, vocal gate enabled)
3. **Auto Return** - After song ends, automatically returns to main menu

## Features

- **Rhythm Gameplay**: Press piano keys in sequence as notes fall down the screen
- **Piano Keyboard Interface**: 8-key piano layout (C, D, E, F, G, A, B, C)
- **Progressive Difficulty**: BPM increases by +10 after 4 consecutive hits, decreases by -10 after 3 consecutive misses
- **Configurable Settings**: Hit window and BPM can be changed via RemoteConfig.json without rebuilding
- **JSON Logging**: Console output for note_hit, note_miss, and song_end events with piano key information
- **SOLID Architecture**: Clean, maintainable code following SOLID principles
- **No MonoBehaviour Dependencies**: Core services are pure C# classes for better testability
- **FPS Counter**: Simple plug-and-play performance monitoring
- **Modular Features**: Pluggable game feature system for easy customization

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
- `IAudioService` - Unified audio system (beat sync, music transport, stem mixing, click track, SFX)
- `IBeatSyncSource` - DSP clock synchronization
- `IMusicTransport` - Song playback control
- `IStemMixer` - Individual stem volume control
- `IClickTrack` - Metronome/beat click playback
- `ISfxService` - Sound effects and instant feedback
- `IConductor` - Song timing and BPM control
- `INoteTimeline` - Note data and queries
- `IInputReader` - Piano key input handling
- `IJudgeService` - Note judgment and hit windows
- `IScoringService` - Score, combo, and streak tracking
- `IEventLogger` - JSON event logging
- `IStaffDrawer` - Musical staff and beat line rendering

### Feature System (`Assets/Scripts/Features/`)
- `IGameFeature` - Pluggable gameplay mechanics interface
- `FeatureManager` - Coordinates and manages game features
- `GameContext` - Provides service references to features
- `VocalGateFeature` - Mutes/unmutes vocal stem based on performance
- `DifficultyScalerFeature` - Adjusts BPM based on hit/miss streaks

### Service Hub (`Assets/Scripts/Gameplay/`)
- `ServiceHub` - Central coordinator for all game services
- **Global Services**: AudioService, EventLogger (initialized once)
- **GamePlay Services**: Conductor, NoteTimeline, InputReader, JudgeService, ScoringService, NoteSpawner, StaffDrawer (rebuilt per level)

### Implementation Classes

#### Pure C# Services (No MonoBehaviour)
- `JudgeService` - Judges note hits based on timing (Pure C# class)
- `ScoringService` - Tracks score, combo, and streaks (Pure C# class)

#### MonoBehaviour Services (Unity Integration)
- `UnityAudioService` - Audio implementation using Unity's audio system
- `Conductor` - Manages song timing and BPM changes (MonoBehaviour)
- `NoteTimeline` - Handles note data and spawning (MonoBehaviour)
- `InputReader` - Processes piano key input (MonoBehaviour)
- `EventLogger` - Logs game events as JSON (MonoBehaviour)
- `GameManager` - Orchestrates all game systems using ServiceHub (MonoBehaviour)
- `NoteSpawner` - Manages visual note objects (MonoBehaviour)
- `NoteObject` - Individual note behavior and visuals (MonoBehaviour)
- `GameUI` - User interface management (MonoBehaviour)
- `GamePlayUI` - In-game UI elements (judgment display, mode/streak info)
- `PianoKeyDisplay` - Visual piano key buttons with event system
- `FPSCounter` - Simple performance monitoring (MonoBehaviour)

## Key Architectural Benefits

- **Mixed Service Types**: Core game logic services (JudgeService, ScoringService) are pure C# classes, while system coordination services (ServiceHub, GameManager) use MonoBehaviour for Unity integration
- **Interface-Based Design**: All core services are defined as interfaces (IAudioService, IConductor, INoteTimeline, etc.), making them easily mockable and testable
- **Better Testability**: Pure C# services can be easily unit tested without Unity dependencies, and interface-based design allows for easy mocking in tests
- **Cleaner Dependencies**: Core services don't inherit from MonoBehaviour, reducing coupling
- **Event-Driven**: All systems communicate through events, maintaining loose coupling
- **Piano-Focused**: Input system designed specifically for piano key rhythm gameplay
- **Modular Features**: Pluggable feature system for easy gameplay customization
- **Service Separation**: Clear distinction between global and gameplay services

## Configuration

### Game Settings (`Assets/StreamingAssets/RemoteConfig.json`)
```json
{
  "initialBPM": 120.0,
  "hitWindow": 0.2,
  "minBPM": 60.0,
  "maxBPM": 200.0
}
```

### Level Configuration (`Assets/Resources/LevelConfigs/`)
- **LevelConfig1.asset** - Rhythm Tap mode (60 BPM, difficulty scaling enabled)
- **LevelConfig2.asset** - Performance mode (54 BPM, vocal gate enabled)

### Global Settings (`Assets/Scripts/Core/GlobalSetting.cs`)
- **UnitsPerBeatBase**: Base movement speed (3f)
- **HitLineX0**: Hit line X position (0f)
- **CleanupBeats**: Note cleanup timing (2.0f)
- **VerticalSpacing**: Note spacing (0.8f)
- **BaseYPosition**: Base Y position (0.0f)

**Note**: GlobalSetting automatically loads from Resources folder on boot and provides static access to these values throughout the project.

### Settings Explained
- `initialBPM`: Starting tempo of the song (now configured per level in LevelConfig)
- `hitWindow`: Time window (in seconds) for hitting notes
- `minBPM`: Minimum BPM limit for difficulty scaling
- `maxBPM`: Maximum BPM limit for difficulty scaling

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
0.0,Hold,1.0,1
8.5,Hold,1.0,1
10.5,Hold,1.0,1
12.5,Hold,1.0,0
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

## Audio System

### Audio Service Architecture
- **IBeatSyncSource**: Provides DSP clock for synchronization
- **IMusicTransport**: Controls song playback (load, play, pause, stop)
- **IStemMixer**: Controls individual stem volumes (vocal, drums, bass, other)
- **IClickTrack**: Provides metronome/beat click playback
- **ISfxService**: Handles instant feedback sounds

### Key Features
- **DSP Time Synchronization**: Perfect audio-visual sync using `AudioSettings.dspTime`
- **Stem Management**: Individual control of vocal, drums, bass, and other stems
- **Click Track**: Optional metronome for beat synchronization
- **Pitch Scaling**: Adjusts audio tempo to match BPM changes
- **Lead-in Timing**: Configurable buffer before song starts (`song.leadInSec`)
- **Schedule Ahead**: Prevents audio glitches (`scheduleAheadSec`)

### Audio Parameters
- **`song.leadInSec`**: Lead-in seconds before downbeat (default: 0.05s)
- **`scheduleAheadSec`**: How far ahead to schedule clicks (default: 0.1s)

## Event Logging

The game logs all important events as JSON to the console with piano key information:

### Note Hit Event
```json
{
  "event_type": "note_hit",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "note_beat": 1.0,
  "note_type": "Hold",
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
  "note_type": "Hold",
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

## UI System

### Game UI Components
- **GameUI**: Main UI management and GamePlayUI reference
- **GamePlayUI**: In-game display (judgment results, game mode, streak)
- **PianoKeyDisplay**: Visual piano key buttons with press/release animations
- **PianoKeyButton**: Individual key animation control
- **PianoKeyEffectController**: Particle effects for key presses
- **HitLineController**: Hit line visual feedback with scale animations

### FPS Counter
- **Plug-and-Play**: Just attach to any GameObject
- **Auto-Setup**: Creates UI elements automatically
- **Simple Display**: Shows current FPS in top-left corner
- **Configurable**: Update interval adjustable in inspector
- **Performance Monitoring**: Essential for development and testing

## Scene Setup

1. **ServiceHub-Based System Creation**: Use ServiceHub to automatically create and manage all game systems
2. **Single Point of Configuration**: GameManager only needs to reference ServiceHub
3. **Automatic System Building**: ServiceHub coordinates all factories and services
4. **Configuration**: Ensure RemoteConfig.json and LevelConfigs are properly set up
5. **Prefab Management**: Create prefabs for each system type with required components

### Required GameObjects:
- **ServiceHub** (with ServiceHub script) - Central coordinator for all systems
- **GameManager** (with GameManager script) - Orchestrates the game using ServiceHub
- **AudioService** (with UnityAudioService script) - Handles all audio functionality
- **FPS Counter** (with FPSCounter script) - Performance monitoring

### ServiceHub Setup:
1. **Global Services**: AudioService, EventLogger (initialized once)
2. **GamePlay Services**: Conductor, NoteTimeline, InputReader, JudgeService, ScoringService, NoteSpawner, StaffDrawer (rebuilt per level)
3. **System Building**: ServiceHub automatically builds all systems when `InitializeGamePlayServices()` is called

### GameManager Setup:
1. **Single Reference**: Only assign ServiceHub reference
2. **Automatic System Access**: GameManager gets all systems via ServiceHub
3. **Feature Management**: Automatically configures VocalGate and DifficultyScaler features

### Benefits:
- **Single Point of Configuration**: GameManager only references ServiceHub
- **Automatic System Management**: ServiceHub handles all service coordination
- **Clean Architecture**: Follows Facade Pattern and Dependency Inversion Principle
- **Easy to Extend**: Can add new features by implementing `IGameFeature`
- **Simplified GameManager**: No more complex factory management code

## How to Use

1. **Setup**: Open the project in Unity 2022.3 LTS or later
2. **Configure**: Modify `RemoteConfig.json` and `LevelConfig` assets to adjust game settings
3. **Assign References**: Drag references in the Inspector (no FindObjectOfType needed!)
4. **Add FPS Counter**: Attach FPSCounter script to any GameObject for performance monitoring
5. **Configure Piano Keys**: Set up piano key mappings in InputReader
6. **Play**: Press Play in Unity or build and run the game
7. **Controls**: 
   - Piano Keys: A, S, D, F, G, H, J, K for different notes
   - Mouse: Click for testing/debugging
   - F1: Toggle FPS counter display
8. **Objective**: Press the correct piano key when notes reach the target line
9. **Scoring**: Build combos for higher scores, maintain accuracy for difficulty increases

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Interfaces and core types
│   ├── Features/       # Pluggable gameplay features
│   ├── Audio/          # Audio system implementation
│   ├── Chart/          # Note timeline and parsing
│   ├── Input/          # Piano key input handling
│   ├── Gameplay/       # Core game systems
│   ├── UI/             # User interface components
│   ├── Logging/        # Event logging
│   └── Staff/          # Musical staff rendering
├── Scenes/             # Unity scenes
├── Resources/          # Level configurations
├── StreamingAssets/    # Configuration and chart files
└── ...
```

## Dependencies

- Unity 2022.3 LTS or later
- TextMeshPro (included with Unity)
- DOTween (for animations)
- No external packages required

## Testing & Testability

The project is designed with testing in mind through interface-based architecture:

### **Interface-Based Design**
- **All core services are interfaces**: `IAudioService`, `IConductor`, `INoteTimeline`, `IInputReader`, etc.
- **Easy mocking**: Services can be easily mocked in unit tests without Unity dependencies
- **Dependency injection**: Components depend on interfaces, not concrete implementations
- **Testable game logic**: Core game mechanics can be tested without Unity runtime

### **Testing Examples**
- **JudgeService**: Pure C# class implementing `IJudgeService` - can test hit window logic independently
- **ScoringService**: Pure C# class implementing `IScoringService` - can test combo/streak logic independently
- **GameManager**: Can be tested with mock services by creating a `TestableGameManager` that takes interfaces as constructor parameters

### **Testing Benefits**
- **Unit testing**: Test individual service logic without Unity
- **Integration testing**: Test service interactions using mock implementations
- **Faster feedback**: No need to run Unity for logic testing
- **CI/CD friendly**: Tests can run in headless environments

## Development

The project is designed to be easily extensible:

- Add new note types by extending the `NoteType` enum
- Implement new judgment systems by extending `IJudgeService`
- Add new scoring mechanics through `IScoringService`
- Create custom charts by following the chart format specification
- Add new features by implementing `IGameFeature`
- **Interface-based services**: Easy to extend and test
- **Mockable dependencies**: All services can be mocked for testing
- Piano key system can be extended for more keys or different instruments

## License

This project is provided as-is for educational and development purposes.
