using UnityEngine;
using Doulingo.Core;
using System;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Service for managing score, combo, and streaks
    /// Follows Single Responsibility Principle by only handling scoring logic
    /// </summary>
    public class ScoringService : IScoringService
    {
        private int perfectScore = 100;
        private int greatScore = 80;
        private int goodScore = 60;
        private int comboMultiplier = 10;
        
        private int currentScore;
        private int currentCombo;
        private int hitStreak;
        private int missStreak;
        private int totalHits;
        private int totalMisses;
        
        public int CurrentScore => currentScore;
        public int TotalScore => currentScore;
        public int CurrentCombo => currentCombo;
        public int HitStreak => hitStreak;
        public int MissStreak => missStreak;
        public int TotalHits => totalHits;
        public int TotalMisses => totalMisses;
        
        public event Action<int> OnScoreChanged;
        public event Action<int> OnComboChanged;
        public event Action<int> OnHitStreakChanged;
        public event Action<int> OnMissStreakChanged;
        
        public void ProcessHit(HitResult result)
        {
            int score = CalculateScore(result);
            currentScore += score;
            
            currentCombo++;
            hitStreak++;
            missStreak = 0;
            totalHits++;
            
            OnScoreChanged?.Invoke(currentScore);
            OnComboChanged?.Invoke(currentCombo);
            OnHitStreakChanged?.Invoke(hitStreak);
            
            Debug.Log($"Hit! Result: {result}, Score: {score}, Combo: {currentCombo}, Hit Streak: {hitStreak}");
        }
        
        public void ProcessMiss()
        {
            currentCombo = 0;
            missStreak++;
            hitStreak = 0;
            totalMisses++;
            
            OnComboChanged?.Invoke(currentCombo);
            OnMissStreakChanged?.Invoke(missStreak);
            
            Debug.Log($"Miss! Combo broken. Miss Streak: {missStreak}");
        }
        
        public void ResetCombo()
        {
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);
        }
        
        public void ResetStreaks()
        {
            hitStreak = 0;
            missStreak = 0;
            OnHitStreakChanged?.Invoke(hitStreak);
            OnMissStreakChanged?.Invoke(missStreak);
        }
        
        public int CalculateScore(HitResult result)
        {
            int baseScore = result switch
            {
                HitResult.Perfect => perfectScore,
                HitResult.Great => greatScore,
                HitResult.Good => goodScore,
                _ => 0
            };
            
            // Add combo bonus
            int comboBonus = (currentCombo * comboMultiplier);
            
            return baseScore + comboBonus;
        }
        
        public void ResetAll()
        {
            currentScore = 0;
            currentCombo = 0;
            hitStreak = 0;
            missStreak = 0;
            totalHits = 0;
            totalMisses = 0;
            
            OnScoreChanged?.Invoke(currentScore);
            OnComboChanged?.Invoke(currentCombo);
            OnHitStreakChanged?.Invoke(hitStreak);
            OnMissStreakChanged?.Invoke(missStreak);
        }
        
        public void SetScoringValues(int perfect, int great, int good, int comboMultiplier)
        {
            perfectScore = perfect;
            greatScore = great;
            goodScore = good;
            this.comboMultiplier = comboMultiplier;
        }
        
        public float GetAccuracyPercentage()
        {
            int total = totalHits + totalMisses;
            return total > 0 ? (float)totalHits / total * 100f : 0f;
        }
    }
}
