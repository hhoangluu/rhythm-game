using UnityEngine;

namespace Doulingo.Setup
{
    /// <summary>
    /// Guide for setting up the Unity scene with the new ServiceHub architecture
    /// </summary>
    public class SceneSetupGuide : MonoBehaviour
    {
        [Header("Scene Setup Instructions")]
        [TextArea(10, 20)]
        [SerializeField] private string setupInstructions = @"
SCENE SETUP GUIDE - Complete Factory Architecture

This project now uses a complete factory pattern for ALL game systems, providing maximum flexibility and consistency.

REQUIRED GAMEOBJECTS:

1. SERVICE HUB (Main Coordinator)
   - Add GameObject with ServiceHub script
   - This is the ONLY thing GameManager needs to reference
   - ServiceHub coordinates ALL factories and creates ALL systems

2. SYSTEM FACTORIES (Create Core Game Systems)
   - ConfigProviderFactory (with ConfigProviderFactory script)
   - ConductorFactory (with ConductorFactory script)  
   - NoteTimelineFactory (with NoteTimelineFactory script)
   - InputReaderFactory (with InputReaderFactory script)

3. SERVICE FACTORIES (Create Pure C# Services)
   - JudgeServiceFactory (with JudgeServiceFactory script)
   - ScoringServiceFactory (with ScoringServiceFactory script)
   - AudioServiceFactory (with AudioServiceFactory script)

4. OTHER SYSTEM FACTORIES (Create Additional Systems)
   - EventLoggerFactory (with EventLoggerFactory script)
   - NoteSpawnerFactory (with NoteSpawnerFactory script)

5. GAME MANAGER (Main Orchestrator)
   - Add GameObject with GameManager script
   - ONLY assign ServiceHub reference
   - GameManager calls serviceHub.BuildFromConfig() to get all systems

SETUP STEPS:

1. Create all required GameObjects with their scripts
2. In ServiceHub, assign ALL factory references (7 total factories)
3. In GameManager, assign ServiceHub reference
4. Create prefabs for each system type if you want to use Instantiate()
5. Note: Difficulty scaling is now handled by the FeatureManager system (DifficultyScalerFeature)

FACTORY ASSIGNMENTS IN SERVICE HUB:

System Factories:
- ConfigProviderFactory → ConfigProviderFactory GameObject
- ConductorFactory → ConductorFactory GameObject
- NoteTimelineFactory → NoteTimelineFactory GameObject
- InputReaderFactory → InputReaderFactory GameObject

Service Factories:
- JudgeServiceFactory → JudgeServiceFactory GameObject
- ScoringServiceFactory → ScoringServiceFactory GameObject
- AudioServiceFactory → AudioServiceFactory GameObject

Other System Factories:
- EventLoggerFactory → EventLoggerFactory GameObject
- NoteSpawnerFactory → NoteSpawnerFactory GameObject

BENEFITS:

- GameManager only knows about ServiceHub
- ServiceHub handles ALL factory coordination
- ALL systems use the same factory pattern
- No more ServiceContainer dependency
- Complete separation of concerns
- Easy to extend with new system types
- Consistent architecture across all systems

The ServiceHub acts as a Facade, hiding the complexity of ALL factories and providing a simple interface to GameManager.
";

        private void Start()
        {
            Debug.Log("Scene Setup Guide loaded. Check the Inspector for detailed instructions.");
        }
    }
}
