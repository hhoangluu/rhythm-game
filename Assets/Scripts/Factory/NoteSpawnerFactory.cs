using UnityEngine;
using Doulingo.Core;
using Doulingo.Gameplay;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating note spawners based on type using prefabs
    /// </summary>
    public class NoteSpawnerFactory : MonoBehaviour
    {
        [Header("Note Spawner Prefabs")]
        [SerializeField] private NoteSpawner noteSpawnerPrefab;
        
        /// <summary>
        /// Gets a note spawner instance based on the specified type
        /// </summary>
        /// <param name="type">Type of note spawner to create</param>
        /// <returns>Note spawner instance</returns>
        public INoteSpawner GetNoteSpawner(NoteSpawnerType type)
        {
            return type switch
            {
                NoteSpawnerType.NoteSpawner => Instantiate(noteSpawnerPrefab),
                _ => Instantiate(noteSpawnerPrefab)
            };
        }
    }
}
