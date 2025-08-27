using UnityEngine;
using Doulingo.Core;
using System.Collections.Generic;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Draws a traditional musical staff with horizontal and vertical lines
    /// </summary>
    public class MusicalStaffDrawer : MonoBehaviour, IStaffDrawer
    {
        [Header("Staff Line Prefab")]
        [SerializeField] private GameObject staffLinePrefab;
        
        [Header("Staff Settings")]
        [SerializeField] private Color staffLineColor = Color.white;
        [SerializeField] private Color barLineColor = Color.white;
        [SerializeField] private int numberOfStaffLines = 5; // Standard 5-line staff
        [SerializeField] private float staffSpacing = 0.2f; // Space between staff lines
        [SerializeField] private float staffWidth = 100f; // Total width of the staff
        
        // Staff parameters
        private float unitsPerBeat;
        private float hitLineX0;
        private float lineHeight;
        private float lineThickness;
        
        // Runtime state
        private bool isInitialized = false;
        private List<GameObject> activeStaffLines = new List<GameObject>();
        private List<GameObject> activeBarLines = new List<GameObject>();
        private bool staffDrawn = false;
        
        public void Initialize(float unitsPerBeat, float hitLineX0, float lineHeight, float lineThickness)
        {
            this.unitsPerBeat = unitsPerBeat;
            this.hitLineX0 = hitLineX0;
            this.lineHeight = lineHeight;
            this.lineThickness = lineThickness;
            
            isInitialized = true;
            
            // Create default staff line prefab if needed
            if (staffLinePrefab == null)
            {
                CreateDefaultStaffLine();
            }
            
            Debug.Log($"[MusicalStaffDrawer] Initialized with unitsPerBeat: {unitsPerBeat}, hitLineX0: {hitLineX0}");
        }
        
        public void DrawStaff()
        {
            if (!isInitialized || staffDrawn) return;
            
            DrawStaffLines();
            DrawBarLines();
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
            
            // Clear all bar lines
            foreach (var line in activeBarLines)
            {
                if (line != null)
                {
                    DestroyImmediate(line);
                }
            }
            activeBarLines.Clear();
            
            staffDrawn = false;
            Debug.Log("[MusicalStaffDrawer] Cleared all staff elements");
        }
        
        public void UpdateStaff()
        {
            // Staff is static, no updates needed
        }
        
        private void DrawStaffLines()
        {
            if (staffLinePrefab == null) return;
            
            // Calculate staff center Y position
            float staffCenterY = 0f;
            float totalStaffHeight = (numberOfStaffLines - 1) * staffSpacing;
            float startY = staffCenterY - totalStaffHeight / 2f;
            
            // Draw horizontal staff lines
            for (int i = 0; i < numberOfStaffLines; i++)
            {
                float y = startY + i * staffSpacing;
                
                // Create staff line
                GameObject staffLine = Instantiate(staffLinePrefab, transform);
                staffLine.transform.localPosition = new Vector3(hitLineX0 + staffWidth / 2f, y, 0f);
                staffLine.transform.localScale = new Vector3(staffWidth, lineThickness, 1f);
                
                // Set color
                var sr = staffLine.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = staffLineColor;
                }
                
                activeStaffLines.Add(staffLine);
            }
            
            Debug.Log($"[MusicalStaffDrawer] Drew {numberOfStaffLines} horizontal staff lines");
        }
        
        private void DrawBarLines()
        {
            if (staffLinePrefab == null) return;
            
            // Calculate staff center Y position
            float staffCenterY = 0f;
            float totalStaffHeight = (numberOfStaffLines - 1) * staffSpacing;
            float startY = staffCenterY - totalStaffHeight / 2f;
            float endY = startY + totalStaffHeight;
            
            // Draw vertical bar lines at regular intervals
            int numberOfBars = Mathf.FloorToInt(staffWidth / unitsPerBeat);
            
            for (int i = 0; i <= numberOfBars; i++)
            {
                float x = hitLineX0 + i * unitsPerBeat;
                
                // Create bar line
                GameObject barLine = Instantiate(staffLinePrefab, transform);
                barLine.transform.localPosition = new Vector3(x, staffCenterY, 0f);
                barLine.transform.localScale = new Vector3(lineThickness, totalStaffHeight + staffSpacing, 1f);
                
                // Set color
                var sr = barLine.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = barLineColor;
                }
                
                activeBarLines.Add(barLine);
            }
            
            Debug.Log($"[MusicalStaffDrawer] Drew {numberOfBars + 1} vertical bar lines");
            staffDrawn = true;
        }
        
        private void CreateDefaultStaffLine()
        {
            // Create a default staff line prefab
            staffLinePrefab = new GameObject("DefaultStaffLine");
            staffLinePrefab.name = "DefaultStaffLine";

            // Add a SpriteRenderer for 2D
            var spriteRenderer = staffLinePrefab.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Create a simple white rectangle sprite
                spriteRenderer.sprite = CreateRectangleSprite();
                spriteRenderer.color = staffLineColor;
            }

            // Set default scale
            staffLinePrefab.transform.localScale = new Vector3(lineThickness, lineHeight, 1f);

            // Hide the template
            staffLinePrefab.SetActive(false);
        }
        
        private Sprite CreateRectangleSprite()
        {
            // Create a simple rectangle texture for staff lines
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
