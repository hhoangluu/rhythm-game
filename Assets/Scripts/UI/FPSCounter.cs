using UnityEngine;
using UnityEngine.UI;

namespace Doulingo.UI
{
    /// <summary>
    /// Simple FPS counter - just attach to any GameObject
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.5f;
        
        private Text fpsText;
        private float deltaTime = 0f;
        private int frameCount = 0;
        
        private void Start()
        {
            CreateFPSText();
        }
        
        private void Update()
        {
            deltaTime += Time.unscaledDeltaTime;
            frameCount++;
            
            if (deltaTime >= updateInterval)
            {
                float fps = frameCount / deltaTime;
                fpsText.text = $"FPS: {fps:F1}";
                
                deltaTime = 0f;
                frameCount = 0;
            }
        }
        
        private void CreateFPSText()
        {
            // Create GameObject
            GameObject textGO = new GameObject("FPS Text");
            
            // Add Canvas if needed
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("FPS Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;
            }
            
            textGO.transform.SetParent(canvas.transform);
            
            // Add Text component
            fpsText = textGO.AddComponent<Text>();
            fpsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            fpsText.fontSize = 32;
            fpsText.color = Color.green;
            fpsText.text = "FPS: 0.0";
            
            // Position
            RectTransform rect = fpsText.rectTransform;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(10, -10);
            rect.sizeDelta = new Vector2(200, 50);
        }
    }
}
