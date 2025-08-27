using UnityEngine;
using System;

namespace Doulingo.Core
{
    public interface IScoringService
    {
        // Properties
        int CurrentScore { get; }
        int TotalScore { get; }
        int CurrentCombo { get; }
        int HitStreak { get; }
        int MissStreak { get; }
        int TotalHits { get; }
        int TotalMisses { get; }
        
        // Events
        event Action<int> OnScoreChanged;
        event Action<int> OnComboChanged;
        event Action<int> OnHitStreakChanged;
        event Action<int> OnMissStreakChanged;
        
        // Methods
        void ProcessHit(HitResult result);
        void ProcessMiss();
        void ResetCombo();
        void ResetStreaks();
        int CalculateScore(HitResult result);
        void ResetAll();
        void SetScoringValues(int perfect, int great, int good, int comboMultiplier);
        float GetAccuracyPercentage();
    }
}
