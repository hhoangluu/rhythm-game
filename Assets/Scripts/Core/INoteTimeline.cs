using System.Collections.Generic;
using UnityEngine;

namespace Doulingo.Core
{
    public interface INoteTimeline
    {
        IReadOnlyList<NoteData> Notes { get; }
        NoteData GetNextNote(float currentBeat);
        NoteData GetNoteAtBeat(float beat);
        bool HasNotesRemaining(float currentBeat);
        void LoadChart(string chartData);
        IReadOnlyList<NoteData> GetNotesInRange(float startBeat, float endBeat);
        float GetLastNoteBeat();

        void Clear();
    }

    [System.Serializable]
    public class NoteData
    {
        public float beat;
        public NoteType type;
        public float duration;
        public int pianoKeyIndex; // Which piano key (0-7) this note corresponds to
        public bool isHited;
        public NoteData(float beat, NoteType type, float duration = 0f, int pianoKeyIndex = 0)
        {
            this.beat = beat;
            this.type = type;
            this.duration = duration;
            this.pianoKeyIndex = pianoKeyIndex;
        }
    }

    public enum NoteType
    {
        Tap,
        Hold,
        Slide
    }
}
