using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Doulingo.Gameplay;
using Doulingo.Core;

namespace Doulingo.UI
{
    /// <summary>
    /// Simple piano key display with colored buttons and click listener
    /// </summary>
    public class PianoKeyDisplay : MonoBehaviour
    {
        [Header("Piano Key Buttons")]
        [SerializeField] private PianoKeyButton[] pianoKeyButtons;

        [Header("Key Colors")]
        [SerializeField] private Color keyCColor = Color.red;      // C = Red
        [SerializeField] private Color keyDColor = Color.green;    // D = Green
        [SerializeField] private Color keyEColor = Color.blue;     // E = Blue
        [SerializeField] private Color keyFColor = Color.yellow;   // F = Yellow
        [SerializeField] private Color keyGColor = Color.magenta;  // G = Magenta
        [SerializeField] private Color keyAColor = Color.cyan;     // A = Cyan
        [SerializeField] private Color keyBColor = Color.white;   // B = Orange
        [SerializeField] private Color keyC2Color = Color.gray;  // C2 = Purple

        // Public events for InputReader to subscribe to
        public event Action<int> OnKeyDown;  // When button is pressed down
        public event Action<int> OnKeyUp;    // When button is released

        [SerializeField] PianoKeyEffectController pianoKeyEffectController;

        private void Start()
        {
            InitializePianoKeys();
        }

        void OnEnable()
        {
            GameManager.OnNoteJudged += HandleNoteJudged;
        }

        void Oisable()
        {
            GameManager.OnNoteJudged -= HandleNoteJudged;
        }

        private void HandleNoteJudged(NoteData note, HitResult result)
        {
            if (result != HitResult.Miss)
            {
                pianoKeyEffectController.ShowEffect(pianoKeyButtons[note.pianoKeyIndex], GetKeyColor(note.pianoKeyIndex));
                pianoKeyButtons[note.pianoKeyIndex].Hit();
            }
        }


        private void InitializePianoKeys()
        {
            if (pianoKeyButtons == null || pianoKeyButtons.Length == 0)
            {
                Debug.LogError("[PianoKeyDisplay] No piano key buttons assigned!");
                return;
            }

            // Set up each button with color and click listener
            for (int i = 0; i < pianoKeyButtons.Length && i < 8; i++)
            {
                if (pianoKeyButtons[i] != null)
                {
                    pianoKeyButtons[i].SetColor(GetKeyColor(i));
                    // Set button color
                    // var buttonImage = pianoKeyButtons[i].GetComponent<Image>();
                    // if (buttonImage != null)
                    // {
                    //     buttonImage.color = GetKeyColor(i);
                    // }

                    // Set up EventTrigger for proper down/up detection
                    SetupButtonEvents(pianoKeyButtons[i], i);

                    Debug.Log($"[PianoKeyDisplay] Initialized key {i} with color {GetKeyColor(i)}");
                }
            }
        }

        private void SetupButtonEvents(PianoKeyButton button, int keyIndex)
        {
            // Get or add EventTrigger component
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            // Clear existing triggers
            eventTrigger.triggers.Clear();

            // Add PointerDown event (button pressed)
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => OnButtonDown(keyIndex));
            eventTrigger.triggers.Add(pointerDown);

            // Add PointerUp event (button released)
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => OnButtonUp(keyIndex));
            eventTrigger.triggers.Add(pointerUp);

            // Add PointerExit event (button released when pointer leaves)
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => OnButtonUp(keyIndex));
            eventTrigger.triggers.Add(pointerExit);
        }

        private Color GetKeyColor(int keyIndex)
        {
            return keyIndex switch
            {
                0 => keyCColor,   // C
                1 => keyDColor,   // D
                2 => keyEColor,   // E
                3 => keyFColor,   // F
                4 => keyGColor,   // G
                5 => keyAColor,   // A
                6 => keyBColor,   // B
                7 => keyC2Color,  // C2
                _ => Color.white
            };
        }


        public void OnButtonDown(int keyIndex)
        {
            Debug.Log($"[PianoKeyDisplay] Key {keyIndex} down");
            pianoKeyButtons[keyIndex].Press();
            OnKeyDown?.Invoke(keyIndex);
        }

        public void OnButtonUp(int keyIndex)
        {
            Debug.Log($"[PianoKeyDisplay] Key {keyIndex} up");
            pianoKeyButtons[keyIndex].Release();
            OnKeyUp?.Invoke(keyIndex);
        }
    }
}
