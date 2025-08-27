using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Interface for spawning and managing note objects
    /// </summary>
    public interface INoteSpawner
    {
        void SpawnNote(NoteData note, float currentBeat);
        bool IsNoteSpawned(NoteData note);
        bool IsNoteProcessed(NoteData note);
        void ProcessNote(NoteData note);
        void Initialize(INoteTimeline noteTimeline, IConductor conductor);
        void Clear();
    }
}
