using UnityEngine;
using Doulingo.Core;
using Doulingo.Chart;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating note timelines based on type using prefabs
    /// </summary>
    public class NoteTimelineFactory : MonoBehaviour
    {
        [Header("Note Timeline Prefabs")]
        [SerializeField] private NoteTimeline noteTimelinePrefab;
        
        /// <summary>
        /// Gets a note timeline instance based on the specified type
        /// </summary>
        /// <param name="type">Type of note timeline to create</param>
        /// <returns>Note timeline instance</returns>
        public INoteTimeline GetNoteTimeline(NoteTimelineType type)
        {
            return type switch
            {
                NoteTimelineType.NoteTimeline => Instantiate(noteTimelinePrefab),
                _ => Instantiate(noteTimelinePrefab)
            };
        }
    }
}
