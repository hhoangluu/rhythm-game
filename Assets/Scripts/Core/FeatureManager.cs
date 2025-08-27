using System.Collections.Generic;
using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Manages and coordinates all pluggable game features
    /// </summary>
    public class FeatureManager
    {
        private readonly List<IGameFeature> features = new List<IGameFeature>();

        /// <summary>
        /// Add a feature to the manager
        /// </summary>
        public void Add(IGameFeature feature)
        {
            if (feature != null && !features.Contains(feature))
            {
                features.Add(feature);
                Debug.Log($"[FeatureManager] Added feature: {feature.GetType().Name}");
            }
        }

        /// <summary>
        /// Remove a feature from the manager
        /// </summary>
        public void Remove(IGameFeature feature)
        {
            if (features.Remove(feature))
            {
                Debug.Log($"[FeatureManager] Removed feature: {feature.GetType().Name}");
            }
        }

        /// <summary>
        /// Initialize all features with the game context
        /// </summary>
        public void InitializeAll(in GameContext ctx)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.Initialize(ctx);
                    Debug.Log($"[FeatureManager] Initialized feature: {feature.GetType().Name}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Failed to initialize feature {feature.GetType().Name}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that the game has started
        /// </summary>
        public void OnGameStart()
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnGameStart();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnGameStart(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that the game has ended
        /// </summary>
        public void OnGameEnd()
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnGameEnd();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnGameEnd(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that a note was hit
        /// </summary>
        public void OnHit(NoteData note, HitResult result, float accuracy)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnHit(note, result, accuracy);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnHit(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that a note was missed
        /// </summary>
        public void OnMiss(NoteData note)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnMiss(note);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnMiss(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that hit streak changed
        /// </summary>
        public void OnHitStreakChanged(int streak)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnHitStreakChanged(streak);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnHitStreakChanged(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Notify all features that miss streak changed
        /// </summary>
        public void OnMissStreakChanged(int streak)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.OnMissStreakChanged(streak);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.OnMissStreakChanged(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Update all features every frame
        /// </summary>
        public void Tick(float dt)
        {
            foreach (var feature in features)
            {
                try
                {
                    feature.Tick(dt);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FeatureManager] Error in {feature.GetType().Name}.Tick(): {e.Message}");
                }
            }
        }

        /// <summary>
        /// Get the number of active features
        /// </summary>
        public int FeatureCount => features.Count;

        /// <summary>
        /// Clear all features
        /// </summary>
        public void Clear()
        {
            features.Clear();
            Debug.Log("[FeatureManager] Cleared all features");
        }
    }
}
