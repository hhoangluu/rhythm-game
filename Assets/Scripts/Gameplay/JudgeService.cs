using UnityEngine;
using Doulingo.Core;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Service for judging note hits based on timing
    /// </summary>
    public class JudgeService : IJudgeService
    {
        private float perfectWindow = 0.05f;
        private float greatWindow = 0.1f;
        private float goodWindow = 0.2f;
        private float hitWindow;
        
        public void SetHitWindow(float window)
        {
            hitWindow = window;
            perfectWindow = window * 0.25f;
            greatWindow = window * 0.5f;
            goodWindow = window;
        }
        
        public HitResult JudgeNote(NoteData note, float currentBeat)
        {
            float timeDifference = Mathf.Abs(currentBeat - note.beat);
            
            if (timeDifference <= perfectWindow)
            {
                return HitResult.Perfect;
            }
            else if (timeDifference <= greatWindow)
            {
                return HitResult.Great;
            }
            else if (timeDifference <= goodWindow)
            {
                return HitResult.Good;
            }
            else
            {
                return HitResult.Miss;
            }
        }
        
        public bool IsNoteInHitWindow(NoteData note, float currentBeat)
        {
            float timeDifference = Mathf.Abs(currentBeat - note.beat);
            return timeDifference <= hitWindow;
        }
        
        public float GetHitAccuracy(NoteData note, float currentBeat)
        {
            float timeDifference = Mathf.Abs(currentBeat - note.beat);
            float maxWindow = hitWindow;
            
            if (timeDifference > maxWindow)
                return 0f;
                
            // Return accuracy as a percentage (0-100)
            return Mathf.Clamp01(1f - (timeDifference / maxWindow)) * 100f;
        }
        
        public void SetJudgmentWindows(float perfect, float great, float good)
        {
            perfectWindow = perfect;
            greatWindow = great;
            goodWindow = good;
            hitWindow = good; // Use good window as the main hit window
        }
        
        public float GetPerfectWindow() => perfectWindow;
        public float GetGreatWindow() => greatWindow;
        public float GetGoodWindow() => goodWindow;
        public float GetHitWindow() => hitWindow;
    }
}
