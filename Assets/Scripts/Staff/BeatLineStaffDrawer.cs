using UnityEngine;
using Doulingo.Core;
using System.Collections.Generic;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Draws beat lines at regular intervals (from NoteSpawner logic)
    /// </summary>
    public class BeatLineStaffDrawer : MonoBehaviour, IStaffDrawer
    {
        [Header("Beat Line Prefab")]
        [SerializeField] private GameObject beatLinePrefab;
        [SerializeField] private GameObject line;

        [Header("Beat Line Settings")]
        [SerializeField] private Color lineColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Staff parameters
        private float unitsPerBeat;
        private float hitLineX0;
        private float lineHeight;
        private float lineThickness;

        // Runtime state
        private bool isInitialized = false;
        private List<BeatLine> activeBeatLines = new List<BeatLine>();
        private bool beatLinesSpawned = false;

        public void Initialize(float unitsPerBeat, float hitLineX0, float lineHeight, float lineThickness)
        {
            this.unitsPerBeat = unitsPerBeat;
            this.hitLineX0 = hitLineX0;
            this.lineHeight = lineHeight;
            this.lineThickness = lineThickness;
            line.transform.position = new Vector3(-5, line.transform.position.y);
            // Todo: make line follow unitPerBeat, Mapping layout in NoteSpawner
            line.transform.parent = Camera.main.transform;
            line.gameObject.SetActive(true);
            isInitialized = true;

            // Create default beat line prefab if needed
            if (beatLinePrefab == null)
            {
                CreateDefaultBeatLine();
            }

            Debug.Log($"[BeatLineStaffDrawer] Initialized with unitsPerBeat: {unitsPerBeat}, hitLineX0: {hitLineX0}");
        }

        public void DrawStaff()
        {
            if (!isInitialized || beatLinesSpawned) return;

            SpawnBeatLines();
        }

        public void ClearStaff()
        {
            line.transform.parent = transform;
            line.gameObject.SetActive(false);
            // Clear all beat lines and return them to pool
            foreach (var beatLine in activeBeatLines)
            {
                if (beatLine != null)
                {
                    ObjectPool<BeatLine>.Return(beatLine);
                }
            }
            activeBeatLines.Clear();
            beatLinesSpawned = false;

            Debug.Log("[BeatLineStaffDrawer] Cleared all beat lines");
        }

        public void UpdateStaff()
        {

        }

        private void SpawnBeatLines()
        {
            if (beatLinePrefab == null) return;

            // Calculate total beats based on a reasonable range
            float totalBeats = 50f; // Adjust as needed

            // Spawn vertical lines at X = hitLineX0 + i * unitsPerBeat
            for (int i = 0; i <= totalBeats; i++)
            {
                float x = hitLineX0 + i * unitsPerBeat;

                // Get beat line from pool
                BeatLine beatLine = ObjectPool<BeatLine>.Get(beatLinePrefab.GetComponent<BeatLine>(), new Vector3(x, 1f, 0f));

                if (beatLine != null)
                {
                    // Set parent and configure directly
                    beatLine.transform.SetParent(transform);

                    // Configure visual properties directly
                    var sr = beatLine.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = lineColor;
                    }
                    beatLine.transform.localScale = new Vector3(lineThickness, lineHeight, 1f);

                    // Track active beat lines
                    activeBeatLines.Add(beatLine);
                }
            }

            beatLinesSpawned = true;
            Debug.Log($"[BeatLineStaffDrawer] Spawned {activeBeatLines.Count} beat lines");
        }

        private void CreateDefaultBeatLine()
        {
            // Create a default beat line prefab for pooling
            beatLinePrefab = new GameObject("DefaultBeatLine");
            beatLinePrefab.name = "DefaultBeatLine";

            // Add a SpriteRenderer for 2D
            var spriteRenderer = beatLinePrefab.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Create a simple white rectangle sprite
                spriteRenderer.sprite = CreateRectangleSprite();
                spriteRenderer.color = lineColor;
            }

            // Add BeatLine component (just for identification and pooling)
            beatLinePrefab.AddComponent<BeatLine>();

            // Set default scale
            beatLinePrefab.transform.localScale = new Vector3(lineThickness, lineHeight, 1f);

            // Hide the template
            beatLinePrefab.SetActive(false);
        }

        private Sprite CreateRectangleSprite()
        {
            // Create a simple rectangle texture for beat lines
            int width = 4;
            int height = 32;
            Texture2D texture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = Color.white;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
}
