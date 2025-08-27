using UnityEngine;
using Doulingo.Core;
using System;
using System.IO;

namespace Doulingo.Logging
{
    public class EventLogger : MonoBehaviour, IEventLogger
    {
        [Header("Logging Settings")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool logToFile = false;
        
        private void Start()
        {
            if (enableLogging)
            {
                Debug.Log("EventLogger initialized - JSON logging enabled");
            }
        }
        
        public void LogNoteHit(NoteData note, HitResult judgment, float accuracy)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "note_hit",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                note_beat = note.beat,
                note_type = note.type.ToString(),
                hit_result = judgment.ToString(),
                accuracy = Mathf.Round(accuracy * 100f) / 100f,
                piano_key_index = note.pianoKeyIndex,
                piano_key_name = GetPianoKeyName(note.pianoKeyIndex),
                lane_number = note.pianoKeyIndex
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        public void LogNoteMiss(NoteData note)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "note_miss",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                note_beat = note?.beat ?? 0f,
                note_type = note?.type.ToString() ?? "Unknown",
                piano_key_index = note?.pianoKeyIndex ?? -1,
                piano_key_name = note != null ? GetPianoKeyName(note.pianoKeyIndex) : "Unknown",
                lane_number = note?.pianoKeyIndex ?? -1
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        public void LogSongEnd(int finalScore, int totalHits, int totalMisses)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "song_end",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                final_score = finalScore,
                total_hits = totalHits,
                total_misses = totalMisses,
                accuracy_percentage = totalHits + totalMisses > 0 ? 
                    Mathf.Round((float)totalHits / (totalHits + totalMisses) * 10000f) / 100f : 0f
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        public void LogDifficultyChange(float oldBPM, float newBPM)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "difficulty_change",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                old_bpm = Mathf.Round(oldBPM * 100f) / 100f,
                new_bpm = Mathf.Round(newBPM * 100f) / 100f,
                bpm_difference = Mathf.Round((newBPM - oldBPM) * 100f) / 100f
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        public void LogBPMChange(float newBPM)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "bpm_change",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                new_bpm = Mathf.Round(newBPM * 100f) / 100f
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        public void LogGameStart(float initialBPM)
        {
            if (!enableLogging) return;
            
            var logData = new
            {
                event_type = "game_start",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                initial_bpm = Mathf.Round(initialBPM * 100f) / 100f
            };
            
            string json = JsonUtility.ToJson(logData, true);
            Debug.Log($"[EVENT] {json}");
            
            if (logToFile)
            {
                LogToFile(json);
            }
        }
        
        private string GetPianoKeyName(int keyIndex)
        {
            return NotePositionUtility.GetPianoKeyName(keyIndex);
        }
        
        private void LogToFile(string json)
        {
            try
            {
                string logPath = Path.Combine(Application.persistentDataPath, "game_events.log");
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {json}\n";
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to write to log file: {e.Message}");
            }
        }
        
        public void SetLoggingEnabled(bool enabled)
        {
            enableLogging = enabled;
        }
        
        public void SetFileLoggingEnabled(bool enabled)
        {
            logToFile = enabled;
        }
        
        public void ClearLogFile()
        {
            try
            {
                string logPath = Path.Combine(Application.persistentDataPath, "game_events.log");
                if (System.IO.File.Exists(logPath))
                {
                    System.IO.File.Delete(logPath);
                    Debug.Log("Log file cleared");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to clear log file: {e.Message}");
            }
        }
    }
}
