using UnityEngine;
using Doulingo.Input;
using Doulingo.UI;
using System;

namespace Doulingo.Input
{
    using Input = UnityEngine.Input; // do not remove
    public class InputReader : MonoBehaviour, IInputReader
    {
        [Header("Piano Input Settings")]
        [SerializeField]
        private KeyCode[] pianoKeys = {
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K
        };

        [Header("UI Reference")]
        [SerializeField] private PianoKeyDisplay pianoKeyDisplay;

        public event Action<int> OnKeyDown;  // keyIndex
        public event Action<int, float> OnKeyUp;  // keyIndex, holdTime

        private bool[] keyStates;
        private bool[] uiKeyStates;

        private float[] keyHoldTimes;

        private void Start()
        {
            // keyStates = new bool[pianoKeys.Length];
            // keyHoldTimes = new float[pianoKeys.Length];
            // uiKeyStates = new bool[pianoKeys.Length];

            // // Validate PianoKeyDisplay reference
            // if (pianoKeyDisplay == null)
            // {
            //     Debug.LogWarning("[InputReader] PianoKeyDisplay reference is missing! Visual feedback won't work.");
            // }
            // else
            // {
            //     // Subscribe to PianoKeyDisplay events
            //     pianoKeyDisplay.OnKeyDown += OnPianoKeyDown;
            //     pianoKeyDisplay.OnKeyUp += OnPianoKeyUp;
            // }
        }

        public void Initialize()
        {
            gameObject.SetActive(true);
            keyStates = new bool[pianoKeys.Length];
            keyHoldTimes = new float[pianoKeys.Length];
            uiKeyStates = new bool[pianoKeys.Length];

            // Validate PianoKeyDisplay reference
            if (pianoKeyDisplay == null)
            {
                Debug.LogWarning("[InputReader] PianoKeyDisplay reference is missing! Visual feedback won't work.");
            }
            else
            {
                // Subscribe to PianoKeyDisplay events
                pianoKeyDisplay.OnKeyDown += OnPianoKeyDown;
                pianoKeyDisplay.OnKeyUp += OnPianoKeyUp;
            }
            // Reset all key states
            for (int i = 0; i < keyStates.Length; i++)
            {
                keyStates[i] = false;
                keyHoldTimes[i] = 0f;
            }
        }

        public void Tick()
        {
            // Update key hold times for pressed keys
            for (int i = 0; i < keyStates.Length; i++)
            {
                if (keyStates[i])
                {
                    keyHoldTimes[i] += Time.deltaTime;
                }
            }
        }

        private void Update()
        {
            Tick();

            for (int i = 0; i < pianoKeys.Length; i++)
            {
                bool isPressed = Input.GetKey(pianoKeys[i]);

                if (isPressed && !keyStates[i])
                {
                    keyStates[i] = true;
                    keyHoldTimes[i] = 0f;
                    pianoKeyDisplay.OnButtonDown(i);
                }
                else if (!isPressed && keyStates[i])
                {
                    keyStates[i] = false;
                    pianoKeyDisplay.OnButtonUp(i);
                }
            }
        }

        // PianoKeyDisplay event handlers
        private void OnPianoKeyDown(int keyIndex)
        {
            if (keyIndex >= 0 && keyIndex < uiKeyStates.Length)
            {
                uiKeyStates[keyIndex] = true;
                keyHoldTimes[keyIndex] = 0f;
                OnKeyDown?.Invoke(keyIndex);
            }
        }

        private void OnPianoKeyUp(int keyIndex)
        {
            if (keyIndex >= 0 && keyIndex < uiKeyStates.Length)
            {
                uiKeyStates[keyIndex] = false;
                OnKeyUp?.Invoke(keyIndex, keyHoldTimes[keyIndex]);
            }
        }

        public void Cleanup()
        {
            // Unsubscribe from PianoKeyDisplay events
            if (pianoKeyDisplay != null)
            {
                pianoKeyDisplay.OnKeyDown -= OnPianoKeyDown;
                pianoKeyDisplay.OnKeyUp -= OnPianoKeyUp;
            }
            pianoKeyDisplay.gameObject.SetActive(false);
        }
    }
}
