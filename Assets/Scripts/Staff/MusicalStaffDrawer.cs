using UnityEngine;
using Doulingo.Core;
using System.Collections.Generic;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Draws a simple musical staff with 5 horizontal lines
    /// </summary>
    public class MusicalStaffDrawer : MonoBehaviour, IStaffDrawer
    {
        [Header("Staff Line Prefab")]
        [SerializeField] private GameObject staffLinePrefab;

        // Runtime state
        private List<GameObject> activeStaffLines = new List<GameObject>();
        private bool staffDrawn = false;

        public void Initialize(float unitsPerBeat, float hitLineX0, float lineHeight, float lineThickness)
        {
            // Create default staff line prefab if needed
            // if (staffLinePrefab == null)
            // {
            //     CreateDefaultStaffLine(lineHeight, lineThickness);
            // }

            Debug.Log("[MusicalStaffDrawer] Initialized");
        }

        public void DrawStaff()
        {
            if (staffDrawn) return;

            DrawStaffLines();
        }

        void Update()
        {
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        }

        public void ClearStaff()
        {
            // Clear all staff lines
            foreach (var line in activeStaffLines)
            {
                if (line != null)
                {
                    DestroyImmediate(line);
                }
            }
            activeStaffLines.Clear();

            staffDrawn = false;
            Debug.Log("[MusicalStaffDrawer] Cleared staff");
        }

        public void UpdateStaff()
        {

        }

        private void DrawStaffLines()
        {
            if (staffLinePrefab == null) return;

            // Draw 5 horizontal staff lines using StaffMapCalculator
            for (int i = 0; i < 5; i++)
            {
                // Convert index to PianoNote (0-4 for 5 lines)
                PianoNote note = (PianoNote)i;
                float y = StaffMapCalculator.GetYPosition(note + 2);

                // Create staff line
                GameObject staffLine = Instantiate(staffLinePrefab, transform);
                staffLine.transform.localPosition = new Vector3(0, y, 0f);
                staffLine.gameObject.SetActive(true);

                activeStaffLines.Add(staffLine);
            }

            Debug.Log("[MusicalStaffDrawer] Drew 5 horizontal staff lines");
            staffDrawn = true;
        }

        // private void CreateDefaultStaffLine(float lineHeight, float lineThickness)
        // {
        //     // Create a default staff line prefab
        //     staffLinePrefab = new GameObject("DefaultStaffLine");
        //     staffLinePrefab.name = "DefaultStaffLine";

        //     // Add a SpriteRenderer for 2D
        //     var spriteRenderer = staffLinePrefab.AddComponent<SpriteRenderer>();
        //     if (spriteRenderer != null)
        //     {
        //         // Create a simple white rectangle sprite
        //         spriteRenderer.sprite = CreateRectangleSprite();
        //     }

        //     // Set default scale
        //     staffLinePrefab.transform.localScale = new Vector3(lineThickness, lineHeight, 1f);

        //     // Hide the template
        //     staffLinePrefab.SetActive(false);
        // }

        // private Sprite CreateRectangleSprite()
        // {
        //     // Create a simple rectangle texture for staff lines
        //     int width = 4;
        //     int height = 32;
        //     Texture2D texture = new Texture2D(width, height);

        //     for (int x = 0; x < width; x++)
        //     {
        //         for (int y = 0; y < height; y++)
        //         {
        //             Color color = Color.white;
        //             texture.SetPixel(x, y, color);
        //         }
        //     }

        //     texture.Apply();
        //     return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        // }
    }
}
