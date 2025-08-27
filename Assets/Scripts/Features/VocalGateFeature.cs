using Doulingo.Core;
using UnityEngine;

namespace Doulingo.Features
{
    /// <summary>
    /// Feature that mutes/unmutes vocal stem based on consecutive hits/misses
    /// </summary>
    public class VocalGateFeature : IGameFeature
    {
        private int missGate = 3;
        private int hitGate = 3;
        private float fadeMs = 500f;

        private GameContext context;
        private bool vocalsMuted = false;

        public void Initialize(in GameContext ctx)
        {
            context = ctx;
            
            // Try to get configuration from GameModeConfig if available
            if (ctx.GameConfig != null)
            {
                // These would come from GameModeConfig in a real implementation
                // For now, we'll use the default values
            }
            
            Debug.Log($"[VocalGateFeature] Initialized with missGate={missGate}, hitGate={hitGate}, fadeMs={fadeMs}");
        }

        /// <summary>
        /// Configure the feature with custom parameters
        /// </summary>
        public void Configure(int missGate, int hitGate, float fadeMs)
        {
            this.missGate = missGate;
            this.hitGate = hitGate;
            this.fadeMs = fadeMs;
            Debug.Log($"[VocalGateFeature] Configured with missGate={missGate}, hitGate={hitGate}, fadeMs={fadeMs}");
        }

        public void OnGameStart()
        {
            // Ensure vocals are unmuted at game start
            if (context.AudioService != null)
            {
                context.AudioService.Unmute("vocal", 0f);
                vocalsMuted = false;
                Debug.Log("[VocalGateFeature] Game started - vocals unmuted");
            }
        }

        public void OnGameEnd()
        {
            // Reset state
            vocalsMuted = false;
            Debug.Log("[VocalGateFeature] Game ended - state reset");
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
            if (context.AudioService == null) return;

            // Unmute vocals if we reach the hit gate
            if (streak >= hitGate && vocalsMuted)
            {
                context.AudioService.Unmute("vocal", fadeMs);
                vocalsMuted = false;
                Debug.Log($"[VocalGateFeature] Hit streak {streak} >= {hitGate} - vocals unmuted");
            }
        }

        public void OnMissStreakChanged(int streak)
        {
            if (context.AudioService == null) return;

            // Mute vocals if we reach the miss gate
            if (streak >= missGate && !vocalsMuted)
            {
                context.AudioService.Mute("vocal", fadeMs);
                vocalsMuted = true;
                Debug.Log($"[VocalGateFeature] Miss streak {streak} >= {missGate} - vocals muted");
            }
        }

        public void Tick(float dt)
        {
            // No continuous updates needed for this feature
        }

        /// <summary>
        /// Get current vocal mute state
        /// </summary>
        public bool IsVocalsMuted => vocalsMuted;

        /// <summary>
        /// Manually set vocal mute state (for testing/debugging)
        /// </summary>
        public void SetVocalsMuted(bool muted, float fadeMs = 0f)
        {
            if (context.AudioService == null) return;

            if (muted && !vocalsMuted)
            {
                context.AudioService.Mute("vocal", fadeMs);
                vocalsMuted = true;
            }
            else if (!muted && vocalsMuted)
            {
                context.AudioService.Unmute("vocal", fadeMs);
                vocalsMuted = false;
            }
        }
    }
}
