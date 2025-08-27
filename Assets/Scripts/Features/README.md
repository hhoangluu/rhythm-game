# Pluggable Feature System

This system allows you to easily add, remove, and configure game features without modifying the core GameManager code.

## Overview

The feature system consists of:
- **`IGameFeature`** - Interface that all features must implement
- **`FeatureManager`** - Coordinates and manages all active features
- **`GameContext`** - Provides features with access to core services
- **`LevelConfig`** - ScriptableObject to configure which features are enabled and their parameters
- **`LevelSelector`** - UI component for selecting different levels

## How to Use

### 1. Create Level Configs

1. Create a `Resources/LevelConfigs/` folder in your project
2. Right-click in the Project window
3. Select `Create > Doulingo > Level Config`
4. Name them exactly as follows:
   - `LevelConfig1` - For level 1 (Beginner)
   - `LevelConfig2` - For level 2 (Expert)
5. Configure which features are enabled and their parameters
6. **Assign SongDescriptor** to each level config

### 2. Set Up Level Selection UI

1. Add **LevelSelector** component to your UI
2. Create buttons for level selection
3. Assign UI references in the inspector
4. The selector will automatically load configs from Resources

### 3. Available Features

#### VocalGateFeature
- **Purpose**: Mutes/unmutes vocal stem based on consecutive hits/misses
- **Parameters**:
  - `missGate`: Number of consecutive misses before muting vocals
  - `hitGate`: Number of consecutive hits before unmuting vocals
  - `fadeMs`: Fade duration in milliseconds

#### DifficultyScalerFeature
- **Purpose**: Adjusts game speed (BPM) based on player performance
- **Parameters**:
  - `upGate`: Number of consecutive hits before increasing difficulty
  - `downGate`: Number of consecutive misses before decreasing difficulty
  - `bpmStep`: BPM change amount
  - `cooldownDuration`: Time between difficulty changes

### 4. Creating Custom Features

To create a new feature:

1. Implement `IGameFeature` interface
2. Add your feature to the `SetupFeatureSystem()` method in GameManager
3. Configure it through LevelConfig if needed

Example:
```csharp
public class MyCustomFeature : IGameFeature
{
    public void Initialize(in GameContext ctx) { /* setup */ }
    public void OnGameStart() { /* called when game starts */ }
    public void OnGameEnd() { /* called when game ends */ }
    public void OnHit(NoteData note, HitResult result, float accuracy) { /* called on note hit */ }
    public void OnMiss(NoteData note) { /* called on note miss */ }
    public void OnHitStreakChanged(int streak) { /* called when hit streak changes */ }
    public void OnMissStreakChanged(int streak) { /* called when miss streak changes */ }
    public void Tick(float dt) { /* called every frame */ }
}
```

## Level Selection Flow

### 1. Level Selection
```
Player clicks Level 1 → Loads "LevelConfig1" from Resources
Player clicks Level 2 → Loads "LevelConfig2" from Resources
```

### 2. Configuration Loading
```
LevelSelector → Resources.Load("LevelConfigs/LevelConfigX")
GameManager.SetLevelConfig(config)
```

### 3. Feature Initialization
```
GameManager.InitializeGame() → SetupFeatureSystem() → Configure features with level settings
```

### 4. Game Start
```
Player clicks Start Game → GameManager.StartGame() → Features activated with level parameters
```

## Benefits

- **Modular**: Features can be added/removed without changing core code
- **Configurable**: Easy to adjust parameters through ScriptableObjects
- **Level-Based**: Different levels with different feature sets and songs
- **Testable**: Each feature can be tested independently
- **Extensible**: New features can be added without modifying existing ones
- **SOLID**: Follows SOLID principles for better architecture

## Architecture

```
LevelSelector (UI)
    ↓
LevelConfig (Resources)
    ↓
GameManager
    ↓
FeatureManager
    ↓
IGameFeature implementations
    ↓
GameContext (provides access to services)
```

## Service Access

Features can access these services through GameContext:
- `IAudioService` - Audio playback and stem management
- `IConductor` - BPM and timing control
- `INoteTimeline` - Note data and queries
- `IConfigProvider` - Configuration access
- `IEventLogger` - Event logging
- `GameConfig` - Game settings and bounds

## Setup Checklist

- [ ] Create `Resources/LevelConfigs/` folder
- [ ] Create `LevelConfig1.asset` (Beginner level)
- [ ] Create `LevelConfig2.asset` (Expert level)
- [ ] Assign SongDescriptor to each level config
- [ ] Set up UI with LevelSelector component
- [ ] Assign UI references in inspector
- [ ] Test level selection and config loading
- [ ] Verify features behave according to level settings
