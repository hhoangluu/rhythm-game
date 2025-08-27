using Doulingo.Factory;
using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Interface for logging game events
    /// </summary>
    public interface IEventLogger
    {
        void LogNoteHit(NoteData note, HitResult judgment, float accuracy);
        void LogNoteMiss(NoteData note);
        void LogSongEnd(int finalScore, int totalHits, int totalMisses);
        void LogBPMChange(float newBPM);
        void LogGameStart(float initialBPM);
        void LogDifficultyChange(float oldBPM, float newBPM);
    }
}
