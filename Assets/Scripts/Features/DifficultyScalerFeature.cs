using Doulingo.Core;
using UnityEngine;
using System;

namespace Doulingo.Features
{
    /// <summary>
    /// Feature that scales gameplay difficulty based on consecutive hits/misses
    /// </summary>
    public class DifficultyScalerFeature : IGameFeature
    {
        private int upGate = 4;
        private int downGate = 3;
        private float bpmStep = 10f;
        private float cooldownDuration = 2f;

        private GameContext context;
        private float cooldownTimer = 0f;
        private bool canChangeDifficulty = true;

        /// <summary>
        /// Event fired when difficulty (BPM) changes
        /// </summary>
        public event Action<float> OnDifficultyChanged;

        public void Initialize(in GameContext ctx)
        {
            context = ctx;
            
            // Try to get configuration from GameModeConfig if available
            if (ctx.GameConfig != null)
            {
                // These would come from GameModeConfig in a real implementation
                // For now, we'll use the default values
            }
            
            Debug.Log($"[DifficultyScalerFeature] Initialized with upGate={upGate}, downGate={downGate}, bpmStep={bpmStep}, cooldown={cooldownDuration}s");
        }

        /// <summary>
        /// Configure the feature with custom parameters
        /// </summary>
        public void Configure(int upGate, int downGate, float bpmStep, float cooldownDuration)
        {
            this.upGate = upGate;
            this.downGate = downGate;
            this.bpmStep = bpmStep;
            this.cooldownDuration = cooldownDuration;
            Debug.Log($"[DifficultyScalerFeature] Configured with upGate={upGate}, downGate={downGate}, bpmStep={bpmStep}, cooldown={cooldownDuration}s");
        }

        public void OnGameStart()
        {
            // Reset cooldown at game start
            cooldownTimer = 0f;
            canChangeDifficulty = true;
            Debug.Log("[DifficultyScalerFeature] Game started - difficulty scaling enabled");
        }

        public void OnGameEnd()
        {
            // Reset state
            cooldownTimer = 0f;
            canChangeDifficulty = true;
            Debug.Log("[DifficultyScalerFeature] Game ended - state reset");
        }

        public void OnHit(NoteData note, HitResult result, float accuracy)
        {
            // This feature only cares about streaks, not individual hits
        }

        public void OnMiss(NoteData note)
        {
            // This feature only cares about streaks, not individual misses
        }

        public void OnHitStreakChanged(int streak)
        {
            if (context.Conductor == null || !canChangeDifficulty) return;

            // Increase difficulty if we reach the up gate
            if (streak >= upGate)
            {
                float currentBPM = context.Conductor.CurrentBPM;
                float newBPM = currentBPM + bpmStep;
                
                // Check if new BPM is within bounds
                if (newBPM <= context.GameConfig.maxBPM)
                {
                    context.Conductor.ChangeBPM(newBPM);
                    OnDifficultyChanged?.Invoke(newBPM);
                    StartCooldown();
                    
                    // CRITICAL FIX: Reset hit streak after BPM change
                    ResetHitStreak();
                    
                    Debug.Log($"[DifficultyScalerFeature] Hit streak {streak} >= {upGate} - BPM increased from {currentBPM} to {newBPM}, streak reset");
                }
                else
                {
                    Debug.Log($"[DifficultyScalerFeature] Hit streak {streak} >= {upGate} - BPM would exceed max ({context.GameConfig.maxBPM})");
                }
            }
        }

        public void OnMissStreakChanged(int streak)
        {
            if (context.Conductor == null || !canChangeDifficulty) return;

            // Decrease difficulty if we reach the down gate
            if (streak >= downGate)
            {
                float currentBPM = context.Conductor.CurrentBPM;
                float newBPM = currentBPM - bpmStep;
                
                // Check if new BPM is within bounds
                if (newBPM >= context.GameConfig.minBPM)
                {
                    context.Conductor.ChangeBPM(newBPM);
                    OnDifficultyChanged?.Invoke(newBPM);
                    StartCooldown();
                    
                    // CRITICAL FIX: Reset miss streak after BPM change
                    ResetMissStreak();
                    
                    Debug.Log($"[DifficultyScalerFeature] Miss streak {streak} >= {downGate} - BPM decreased from {currentBPM} to {newBPM}, streak reset");
                }
                else
                {
                    Debug.Log($"[DifficultyScalerFeature] Miss streak {streak} >= {downGate} - BPM would go below min ({context.GameConfig.minBPM})");
                }
            }
        }
        
        // Add these new methods to reset streaks
        private void ResetHitStreak()
        {
            // Reset hit streak through scoring service
            if (context.ScoringService != null)
            {
                context.ScoringService.ResetStreaks();
                Debug.Log("[DifficultyScalerFeature] Hit streak reset after BPM increase");
            }
        }
        
        private void ResetMissStreak()
        {
            // Reset miss streak through scoring service
            if (context.ScoringService != null)
            {
                context.ScoringService.ResetStreaks();
                Debug.Log("[DifficultyScalerFeature] Miss streak reset after BPM decrease");
            }
        }

        public void Tick(float dt)
        {
            // Update cooldown timer
            if (!canChangeDifficulty)
            {
                cooldownTimer -= dt;
                if (cooldownTimer <= 0f)
                {
                    canChangeDifficulty = true;
                    Debug.Log("[DifficultyScalerFeature] Cooldown expired - difficulty scaling re-enabled");
                }
            }
        }

        private void StartCooldown()
        {
            canChangeDifficulty = false;
            cooldownTimer = cooldownDuration;
            Debug.Log($"[DifficultyScalerFeature] Started cooldown for {cooldownDuration}s");
        }

        /// <summary>
        /// Get current difficulty change availability
        /// </summary>
        public bool CanChangeDifficulty => canChangeDifficulty;

        /// <summary>
        /// Get remaining cooldown time
        /// </summary>
        public float RemainingCooldown => Mathf.Max(0f, cooldownTimer);

        /// <summary>
        /// Manually reset cooldown (for testing/debugging)
        /// </summary>
        public void ResetCooldown()
        {
            canChangeDifficulty = true;
            cooldownTimer = 0f;
            Debug.Log("[DifficultyScalerFeature] Cooldown manually reset");
        }
    }
}
