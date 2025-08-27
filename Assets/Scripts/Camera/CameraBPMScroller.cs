using System;
using Doulingo.Core;
using UnityEngine;

namespace Doulingo.Gameplay
{
    /// <summary>
    /// Di chuyển camera sang phải theo beat của Conductor.
    /// Mapping: 1 beat = unitsPerBeat world units.
    /// Gợi ý: Đặt script vào 1 empty "CameraRig" làm parent của Camera chính.
    /// </summary>
    public class CameraBpmScroller : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Camera sẽ được di chuyển. Để trống sẽ auto dùng Camera.main.")]
        [SerializeField] private Transform targetCamera;

        // Interface runtime
        private IConductor conductor;

        [Header("Movement Mapping")]
        [Tooltip("1 beat dịch bao nhiêu world units (trục X).")]
        [SerializeField] private float unitsPerBeat = 3f;

        [Tooltip("Offset beat lúc bắt đầu. Ví dụ spawn trước 4 beat => đặt -4 để camera bắt đầu lệch trái.")]
        [SerializeField] private float startBeatOffset = 0f;

        private Vector3 startPos;
        private bool isInitialized = false;
        private void Awake()
        {
            if (!targetCamera)
            {
                var cam = Camera.main;
                if (cam) targetCamera = cam.transform;

            }

            if (targetCamera == null)
                Debug.LogError("[CameraBpmScroller] targetCamera chưa gán và không tìm thấy Camera.main.");

            if (targetCamera != null) startPos = targetCamera.position;
        }

        private void Start()
        {
            // Subscribe to the initialization event
            GameManager.OnInitializeSystemsComplete += OnGameSystemsInitialized;

            // Subscribe to song end event
            GameManager.OnSongEnd += OnSongEnd;
            GameManager.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            TryGetConductor();
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event
            GameManager.OnInitializeSystemsComplete -= OnGameSystemsInitialized;
            GameManager.OnSongEnd -= OnSongEnd;
            GameManager.OnGameStart -= OnGameStart;
        }

        private void OnGameSystemsInitialized()
        {
            Debug.Log("[CameraBpmScroller] Game systems initialized, getting conductor...");
            TryGetConductor();
        }

        private void OnSongEnd()
        {
            conductor = null;
            // Reset camera position when song ends
            ResetStartPositionToCurrent();
            Debug.Log("[CameraBpmScroller] Song ended, camera position reset");
        }

        private void TryGetConductor()
        {
            // Try to get conductor from ServiceHub
            conductor = ServiceHub.Conductor;
            if (conductor != null)
            {
                isInitialized = true;
                Debug.Log("[CameraBpmScroller] Conductor obtained successfully!");
            }
            else
            {
                Debug.LogWarning("[CameraBpmScroller] Conductor not available yet, waiting for initialization...");
            }
        }

        void Update()
        {
            if (conductor == null || !conductor.IsPlaying) return;

            float beat = conductor.SongPositionInBeats;
            float x = startPos.x + (beat + startBeatOffset) * unitsPerBeat;

            targetCamera.position = new Vector3(x, startPos.y, startPos.z);
        }


        /// <summary>
        /// Đặt lại vị trí bắt đầu (ví dụ khi restart bài).
        /// </summary>
        public void ResetStartPositionToCurrent()
        {
            if (targetCamera != null) targetCamera.position = startPos;
        }
    }
}