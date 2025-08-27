using UnityEngine;
using Doulingo.Core;
using System;

namespace Doulingo.Audio
{
    public class Conductor : MonoBehaviour, IConductor
    {
        [Header("Conductor Settings")]
        [SerializeField] private float songStartDelay = 1f;

        private float currentBPM;
        private float songPositionSec;
        private float lastBeatTime;
        private bool isPlaying;
        private double songStartDspTime;
        private float songStartTime;
        private IBeatSyncSource beatSyncSource;
        private float totalBeats;

        // Song end detection
        private float endDelayBeats = 4f; // Wait 2 beats after last note

        public float CurrentBPM => currentBPM;
        public float SongPositionInBeats
        {
            get
            {
                double dspNow = beatSyncSource?.DspTime ?? AudioSettings.dspTime;
                double elapsed = dspNow - songStartDspTime;
                return (float)(elapsed * currentBPM / 60.0);
            }
        }
        public float SongPositionSec => songPositionSec;
        public float BeatDuration => 60f / currentBPM;
        public bool IsPlaying => isPlaying;

        public event Action<float> OnBPMChanged;
        public event Action OnSongStarted;
        public event Action OnSongEnded;

        public void Initialize(float initialBPM, float songStartDelay, float totalBeats, IBeatSyncSource beatSyncSource)
        {
            ResetConductor();
            this.currentBPM = initialBPM;
            this.songStartDelay = songStartDelay;
            this.beatSyncSource = beatSyncSource;
            this.totalBeats = totalBeats;
        }

        private void Update()
        {
            if (isPlaying)
            {
               // UpdateSongPosition(Time.deltaTime);
               
               // Check for song end
               CheckSongEnd();
            }
        }

        private void CheckSongEnd()
        {
            if (isPlaying)
            {
                float currentBeat = SongPositionInBeats;
                if (currentBeat >= totalBeats + endDelayBeats)
                {
                    Debug.Log($"[Conductor] Song end detected at beat {currentBeat:F2} (end beat: {totalBeats:F2})");
                    currentBeat = 0;
                    isPlaying = false;
                    OnSongEnded?.Invoke();
                }
            }
        }

        public void StartSong()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                songStartDspTime = AudioSettings.dspTime + songStartDelay;
                songStartTime = Time.time + songStartDelay;
                lastBeatTime = songStartTime;
                OnSongStarted?.Invoke();
                Debug.Log($"Song started at BPM: {currentBPM}, DSP start: {songStartDspTime}");
            }
        }

        public void StopSong()
        {
            if (isPlaying)
            {
                isPlaying = false;
                OnSongEnded?.Invoke();
                Debug.Log("Song stopped");
            }
        }

        public void ChangeBPM(float newBPM)
        {
            if (Mathf.Abs(newBPM - currentBPM) > 0.1f)
            {
                float oldBPM = currentBPM;

                if (beatSyncSource != null && isPlaying)
                {
                    double currentDsp = beatSyncSource.DspTime;

                    // Tính vị trí beat hiện tại theo BPM cũ
                    double elapsedOld = currentDsp - songStartDspTime;
                    double currentBeat = elapsedOld * currentBPM / 60.0;

                    // Gán BPM mới
                    currentBPM = newBPM;

                    // Điều chỉnh mốc start để giữ nguyên beat
                    songStartDspTime = currentDsp - (currentBeat * 60.0 / currentBPM);

                    Debug.Log($"[Conductor] BPM changed to {newBPM}, adjusted songStartDspTime to preserve beat position.");
                }

                currentBPM = newBPM;

                // Fire BPM change event immediately
                OnBPMChanged?.Invoke(currentBPM);

                Debug.Log($"BPM changed from {oldBPM} to {newBPM}, new BeatDuration: {BeatDuration:F3}s, beat position preserved at {SongPositionInBeats:F2}");
            }
        }

        public void UpdateSongPosition(float deltaTime)
        {
            if (!isPlaying) return;

            // CRITICAL FIX: Use DSP time consistently for both audio and visual sync
            if (beatSyncSource != null)
            {
                double currentDspTime = beatSyncSource.DspTime;

                // Check if we've passed the song start delay
                if (currentDspTime < songStartDspTime) return;

                double elapsedDspTime = currentDspTime - songStartDspTime;
                songPositionSec = (float)elapsedDspTime;

                // Calculate beats directly from DSP time for perfect audio-visual sync
             //   songPositionInBeats = (float)(elapsedDspTime * currentBPM / 60.0);

                // Update lastBeatTime for beat events
                float timeSinceLastBeat = (float)(elapsedDspTime - (lastBeatTime - songStartTime));
                if (timeSinceLastBeat >= BeatDuration)
                {
                    lastBeatTime = Time.time;
                }
            }
            else
            {
                // Fallback to Unity time (less precise)
                if (Time.time < songStartTime) return;

                float elapsedTime = Time.time - songStartTime;
                songPositionSec = elapsedTime;
            //    songPositionInBeats = elapsedTime * currentBPM / 60f;

                // Update lastBeatTime for beat events
                float timeSinceLastBeat = elapsedTime - (lastBeatTime - songStartTime);
                if (timeSinceLastBeat >= BeatDuration)
                {
                    lastBeatTime = Time.time;
                }
            }
        }

        private void ResetConductor()
        {
          //  songPositionInBeats = 0f;
            songPositionSec = 0f;
            lastBeatTime = 0f;
            isPlaying = false;
            totalBeats = 0;
        }
    }
}