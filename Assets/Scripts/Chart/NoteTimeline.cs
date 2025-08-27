using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Doulingo.Core;

namespace Doulingo.Chart
{
    public class NoteTimeline : MonoBehaviour, INoteTimeline
    {
        [Header("Chart Settings")]
        [SerializeField] private TextAsset defaultChart;
        
        private List<NoteData> notes = new List<NoteData>();
        private int currentNoteIndex = 0;
        
        public IReadOnlyList<NoteData> Notes => notes.AsReadOnly();
        
        // Note: Chart loading is now handled explicitly by GameManager
        // The defaultChart field is kept for potential future use
        
        public NoteData GetNextNote(float currentBeat)
        {
            if (currentNoteIndex >= notes.Count)
                return null;
                
            NoteData nextNote = notes[currentNoteIndex];
            if (nextNote.beat <= currentBeat + 1f) // Look ahead by 1 beat
            {
                currentNoteIndex++;
                return nextNote;
            }
            
            return null;
        }
        
        public NoteData GetNoteAtBeat(float beat)
        {
            return notes.FirstOrDefault(note => Mathf.Abs(note.beat - beat) < 0.1f);
        }
        
        public bool HasNotesRemaining(float currentBeat)
        {
            return currentNoteIndex < notes.Count && 
                   notes[currentNoteIndex].beat <= currentBeat + 2f; // Look ahead by 2 beats
        }
        
        public IReadOnlyList<NoteData> GetNotesInRange(float startBeat, float endBeat)
        {
            return notes.Where(note => note.beat >= startBeat && note.beat <= endBeat).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the beat position of the last note in the chart
        /// </summary>
        public float GetLastNoteBeat()
        {
            if (notes.Count == 0) return 0f;
            return notes[notes.Count - 1].beat;
        }
        
        public void LoadChart(string chartData)
        {
            notes.Clear();
            currentNoteIndex = 0;
            
            // If no chart data provided, try to use defaultChart or create default
            if (string.IsNullOrEmpty(chartData))
            {
                if (defaultChart != null)
                {
                    Debug.Log("No chart data provided, using defaultChart TextAsset");
                    LoadChart(defaultChart.text);
                    return;
                }
                else
                {
                    Debug.Log("No chart data provided and no defaultChart assigned, creating default chart");
                    CreateDefaultChart();
                    return;
                }
            }
            
            try
            {
                Debug.Log($"Loading chart data: {chartData.Length} characters");
                
                // Chart format: "beat,type,duration,pianoKeyIndex" (positions calculated automatically)
                string[] lines = chartData.Split('\n');
                Debug.Log($"Split into {lines.Length} lines");
                
                int validLines = 0;
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        Debug.Log($"Skipping line: '{line}'");
                        continue;
                    }
                        
                    string[] parts = line.Split(',');
                    Debug.Log($"Line '{line}' has {parts.Length} parts: [{string.Join(", ", parts)}]");
                    
                    if (parts.Length >= 3)
                    {
                        float beat = float.Parse(parts[0]);
                        NoteType type = (NoteType)System.Enum.Parse(typeof(NoteType), parts[1]);
                        float duration = parts.Length > 2 ? float.Parse(parts[2]) : 0f;
                        
                        int pianoKeyIndex = 0;
                        if (parts.Length >= 4)
                        {
                            pianoKeyIndex = int.Parse(parts[3]);
                        }
                        
                        notes.Add(new NoteData(beat, type, duration, pianoKeyIndex));
                        validLines++;
                        Debug.Log($"Added note: beat={beat}, type={type}, duration={duration}, key={pianoKeyIndex}");
                    }
                    else
                    {
                        Debug.LogWarning($"Line '{line}' has insufficient parts ({parts.Length}), skipping");
                    }
                }
                
                notes = notes.OrderBy(note => note.beat).ToList();
                Debug.Log($"Chart loaded with {notes.Count} notes from {validLines} valid lines");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse chart: {e.Message}");
                CreateDefaultChart();
            }
        }
        
        private void CreateDefaultChart()
        {
            notes.Clear();
            currentNoteIndex = 0;
            
            // Create a simple test chart with piano keys
            for (int i = 0; i < 20; i++)
            {
                float beat = i * 1f; // One note per beat
                int pianoKeyIndex = i % 8; // Cycle through piano keys 0-7
                
                notes.Add(new NoteData(beat, NoteType.Tap, 0f, pianoKeyIndex));
            }
            
            Debug.Log("Default chart created with 20 notes and piano key mapping");
        }
        
        public void Clear()
        {
            notes.Clear();
            currentNoteIndex = 0;
        }
    }
}
