using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Common interface for all pluggable game features
    /// </summary>
    public interface IGameFeature
    {
        /// <summary>
        /// Initialize the feature with game context
        /// </summary>
        void Initialize(in GameContext ctx);

        /// <summary>
        /// Called when the game starts
        /// </summary>
        void OnGameStart();

        /// <summary>
        /// Called when the game ends
        /// </summary>
        void OnGameEnd();

        /// <summary>
        /// Called when a note is hit
        /// </summary>
        void OnHit(NoteData note, HitResult result, float accuracy);

        /// <summary>
        /// Called when a note is missed
        /// </summary>
        void OnMiss(NoteData note);

        /// <summary>
        /// Called when hit streak changes
        /// </summary>
        void OnHitStreakChanged(int streak);

        /// <summary>
        /// Called when miss streak changes
        /// </summary>
        void OnMissStreakChanged(int streak);

        /// <summary>
        /// Called every frame for continuous updates
        /// </summary>
        void Tick(float dt);
    }
}
