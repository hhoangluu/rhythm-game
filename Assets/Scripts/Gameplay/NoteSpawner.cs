using UnityEngine;
using Doulingo.Core;
using Doulingo.Chart;
using System.Collections.Generic;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// NoteSpawner: Spawns notes at fixed 2D positions mapped from beats.
    /// Notes are arranged like a musical staff: low notes lower, high notes higher.
    /// The camera moves from left to right across the notes.
    /// Uses generic ObjectPool for better performance and reusability.
    /// </summary>
    public class NoteSpawner : MonoBehaviour, INoteSpawner
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject notePrefab;

        [Header("Piano Note Colors")]
        [SerializeField] private Color noteCColor = Color.red;      // C = Red
        [SerializeField] private Color noteDColor = Color.green;    // D = Green
        [SerializeField] private Color noteEColor = Color.blue;     // E = Blue
        [SerializeField] private Color noteFColor = Color.yellow;   // F = Yellow
        [SerializeField] private Color noteGColor = Color.magenta;  // G = Magenta
        [SerializeField] private Color noteAColor = Color.cyan;     // A = Cyan
        [SerializeField] private Color noteBColor = Color.white;   // B = Orange
        [SerializeField] private Color noteC2Color = Color.gray;  // C2 = Purple

        // References
        private INoteTimeline noteTimeline;
        private IConductor conductor;

        // Runtime state
        private readonly List<NoteObject> activeNoteObjects = new List<NoteObject>();
        private readonly HashSet<NoteData> spawnedNotes = new HashSet<NoteData>();
        private readonly HashSet<NoteData> processedNotes = new HashSet<NoteData>();

        [SerializeField]
        HitLineController hitLineController;

        /// <summary>
        /// Get the color for a specific piano key index
        /// </summary>
        private Color GetNoteColor(int keyIndex)
        {
            return keyIndex switch
            {
                0 => noteCColor,   // C
                1 => noteDColor,   // D
                2 => noteEColor,   // E
                3 => noteFColor,   // F
                4 => noteGColor,   // G
                5 => noteAColor,   // A
                6 => noteBColor,   // B
                7 => noteC2Color,  // C2
                _ => Color.white   // Default
            };
        }

        public void Initialize(INoteTimeline timeline, IConductor conductor)
        {
            this.noteTimeline = timeline;
            this.conductor = conductor;

            // Create default note if needed (ObjectPool will auto-configure when first used)
            if (notePrefab == null)
            {
                CreateDefaultNote();
            }

        }

        private void Update()
        {
            if (noteTimeline == null || conductor == null) return;
            CleanupPassedNotes();
        }

        private void SpawnNoteBeatLocked(NoteData note)
        {
            // Get note from generic pool (auto-configures if needed)
            var noteComponent = notePrefab.GetComponent<NoteObject>();
            NoteObject noteComp = ObjectPool<NoteObject>.Get(noteComponent, Vector3.zero);

            if (noteComp == null)
            {
                Debug.LogError("[NoteSpawner] Failed to get note from pool!");
                return;
            }

            // Compute 2D position: X = beat progression, Y = musical staff height
            float y = StaffMapCalculator.GetYPosition((PianoNote)note.pianoKeyIndex);
            float x = GlobalSetting.HitLineX0 + note.beat * GlobalSetting.UnitsPerBeatBase; // X-axis for beat progression

            // Create position directly as Vector3 (Z = 0 for 2D)
            Vector3 spawnPos = new Vector3(x, y, 0f);

            // Position and activate the pooled note
            noteComp.gameObject.transform.position = spawnPos;

            // Initialize the note with data
            noteComp.Initialize(note);

            // Set the note color based on piano key
            Color noteColor = GetNoteColor(note.pianoKeyIndex);
            noteComp.SetColor(noteColor);
            activeNoteObjects.Add(noteComp);
            noteComp.gameObject.SetActive(true);
            spawnedNotes.Add(note);

            Debug.Log($"[NoteSpawner] Spawned note at beat {note.beat}, lane {note.pianoKeyIndex}, pos2D=({spawnPos.x:F1}, {spawnPos.y:F1})");
        }

        private void CreateDefaultNote()
        {
            // Create a 2D sprite as default prefab (better for 2D games)
            notePrefab = new GameObject("DefaultNote");
            notePrefab.name = "DefaultNote";

            // Add a SpriteRenderer for 2D
            var spriteRenderer = notePrefab.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Create a simple white circle sprite
                spriteRenderer.sprite = CreateCircleSprite();
                spriteRenderer.color = Color.white;
            }

            // Scale for 2D
            notePrefab.transform.localScale = Vector3.one * 0.5f;

            // Ensure NoteObject component
            if (notePrefab.GetComponent<NoteObject>() == null)
                notePrefab.AddComponent<NoteObject>();

            // Hide the template
            notePrefab.SetActive(false);
        }

        private Sprite CreateCircleSprite()
        {
            // Create a simple circle texture for 2D notes
            int size = 32;
            Texture2D texture = new Texture2D(size, size);

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    Color color = distance <= radius ? Color.white : Color.clear;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        private void CleanupPassedNotes()
        {
            float curBeat = conductor.SongPositionInBeats;

            // Cleanup notes
            for (int i = activeNoteObjects.Count - 1; i >= 0; i--)
            {
                var noteObj = activeNoteObjects[i];
                if (noteObj == null)
                {
                    activeNoteObjects.RemoveAt(i);
                    continue;
                }

                // Beat-based cleanup: note must pass hit line + cleanupBeats
                float beatAge = curBeat - noteObj.NoteData.beat;
                bool shouldCleanup = beatAge > GlobalSetting.CleanupBeats;

                if (shouldCleanup)
                {
                    activeNoteObjects.RemoveAt(i);
                    spawnedNotes.Remove(noteObj.NoteData);

                    // Return to generic pool (no prefab reference needed)
                    ObjectPool<NoteObject>.Return(noteObj);

                    Debug.Log($"[NoteSpawner] Cleaned up note at beat {noteObj.NoteData.beat}, lane {noteObj.NoteData.pianoKeyIndex}");
                }
            }

            // // Cleanup beat lines
            // for (int i = activeBeatLines.Count - 1; i >= 0; i--)
            // {
            //     var beatLine = activeBeatLines[i];
            //     if (beatLine == null)
            //     {
            //         activeBeatLines.RemoveAt(i);
            //         continue;
            //     }

            //     // Beat-based cleanup: beat line must pass hit line + cleanupBeats
            //     // Calculate beat position from X position
            //     float beatPosition = (beatLine.transform.position.x - hitLineX0) / unitsPerBeatBase;
            //     float beatAge = curBeat - beatPosition;
            //     bool shouldCleanup = beatAge > cleanupBeats;

            //     if (shouldCleanup)
            //     {
            //         activeBeatLines.RemoveAt(i);

            //         // Return to pool
            //         ObjectPool<BeatLine>.Return(beatLine);

            //         Debug.Log($"[NoteSpawner] Cleaned up beat line at beat {beatPosition:F1}");
            //     }
            // }
        }

        public void HitNote(NoteData noteData)
        {
            NoteObject noteObj = FindNoteObject(noteData);
            if (noteObj != null)
            {
                noteObj.Hit();
                // Remove immediate cleanup - let automatic cleanup handle it after cleanupBeats
                // StartCoroutine(RemoveNoteAfterAnimation(noteObj));
            }
        }

        public void MissNote(NoteData noteData)
        {
            NoteObject noteObj = FindNoteObject(noteData);
            if (noteObj != null)
            {
                noteObj.Miss();
                // Remove immediate cleanup - let automatic cleanup handle it after cleanupBeats
                // StartCoroutine(RemoveNoteAfterAnimation(noteObj));
            }
        }

        private NoteObject FindNoteObject(NoteData noteData)
        {
            return activeNoteObjects.Find(n => n.NoteData == noteData);
        }

        public void Clear()
        {
            var noteComponent = notePrefab.GetComponent<NoteObject>();

            foreach (var noteObj in activeNoteObjects)
            {
                if (noteObj != null)
                {
                    ObjectPool<NoteObject>.Return(noteObj);
                }
            }

            activeNoteObjects.Clear();
            spawnedNotes.Clear();

            // Also clear beat lines
            // ClearAllBeatLines();
        }

        /// <summary>
        /// Clear all beat lines and return them to the pool
        /// </summary>
        // public void ClearAllBeatLines()
        // {
        //     foreach (var beatLine in activeBeatLines)
        //     {
        //         if (beatLine != null)
        //         {
        //             ObjectPool<BeatLine>.Return(beatLine);
        //         }
        //     }

        //     activeBeatLines.Clear();
        //     beatLinesSpawned = false;

        //     Debug.Log("[NoteSpawner] Cleared all beat lines and returned them to pool");
        // }

        // ===== Interface Implementation =====
        public void SpawnNote(NoteData note, float currentBeat)
        {
            // Forward to beat-locked spawn method
            SpawnNoteBeatLocked(note);
        }

        public bool IsNoteSpawned(NoteData note) => spawnedNotes.Contains(note);
        public bool IsNoteProcessed(NoteData note) => processedNotes.Contains(note);

        public void ProcessNote(NoteData note)
        {
            processedNotes.Add(note);
            var noteObj = FindNoteObject(note);

            if (note.isHited)
            {
                if (noteObj != null)
                {
                    noteObj.Hit();
                }
            }
            else
            {
                noteObj.Skip();
            }
        }

        // public void SpawnBeatLines()
        // {
        //     if (beatLinesSpawned || noteTimeline == null) return;

        //     EnsureBeatLinePrefab();

        //     // Compute total beats = ceil(max(startBeat + duration))
        //     int totalBeats = 0;
        //     // If your timeline exposes the list directly; adjust accessor name if needed
        //     foreach (var n in noteTimeline.Notes)
        //     {
        //         int endBeat = Mathf.CeilToInt(n.beat + Mathf.Max(0f, n.duration)); // duration in beats
        //         if (endBeat > totalBeats) totalBeats = endBeat;
        //     }

        //     // // Spawn vertical lines at X = hitLineX0 + i * unitsPerBeatBase
        //     // for (int i = 0; i <= totalBeats; i++)
        //     // {
        //     //     float x = hitLineX0 + i * unitsPerBeatBase;

        //     //     // Get beat line from pool
        //     //     BeatLine beatLine = ObjectPool<BeatLine>.Get(beatLinePrefab.GetComponent<BeatLine>(), new Vector3(x, 0f, 0f));

        //     //     if (beatLine != null)
        //     //     {
        //     //         // Set parent and configure directly
        //     //         beatLine.transform.SetParent(transform);

        //     //         // Configure visual properties directly
        //     //         var sr = beatLine.GetComponent<SpriteRenderer>();
        //     //         if (sr != null)
        //     //         {
        //     //             sr.color = lineColor;
        //     //         }
        //     //         beatLine.transform.localScale = new Vector3(lineThickness, lineHeight, 1f);

        //     //         // Track active beat lines
        //     //         activeBeatLines.Add(beatLine);
        //     //     }
        //     // }

        //     // beatLinesSpawned = true;
        //     // Debug.Log($"[NoteSpawner] Spawned {activeBeatLines.Count} beat lines using object pooling");
        // }

    }
}
