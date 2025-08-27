using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Song Descriptor", fileName = "SongDescriptor")]
public sealed class SongDescriptor : ScriptableObject
{
    [Header("Identity")]
    public string songId = "demo";                 // unique key
    public string displayName = "Demo Song";
    public string artist = "Unknown";

    [Header("Tempo")]
    [Tooltip("Base BPM (constant) used by Conductor. Audio runs at original tempo.")]
    public float bpmBase = 100f;
    [Tooltip("Lead-in seconds before downbeat; used for PlayScheduled alignment.")]
    public double leadInSec = 0.05;

    [Header("Audio Sources (choose one path)")]
    [Tooltip("If set and Stems list is empty -> play this full mix.")]
    public AudioClip fullMix;

    [System.Serializable]
    public class StemEntry
    {
        [Tooltip("Id: vocal, drums, bass, other, beat ...")]
        public string id;
        public AudioClip clip;
    }
    [Tooltip("If non-empty -> play stems in sync (recommended ids: vocal/drums/bass/other/beat).")]
    public List<StemEntry> stems = new();

    [Header("Optional")]
    [Tooltip("Click/metronome sound used for beat track when enabled.")]
    public AudioClip clickClip;
    [Tooltip("Human-friendly tags, e.g., training, easy, pop.")]
    public string[] tags;

    // ---------- Convenience helpers ----------
    public bool HasStems => stems != null && stems.Count > 0;
    public bool HasFullMix => fullMix != null;

    public bool TryGetStem(string id, out AudioClip clip)
    {
        if (stems != null)
        {
            for (int i = 0; i < stems.Count; i++)
                if (stems[i].id == id) { clip = stems[i].clip; return clip; }
        }
        clip = null; return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (bpmBase <= 0f) bpmBase = 100f;
        if (leadInSec < 0) leadInSec = 0;
        // Deduplicate/trim stem ids
        if (stems != null)
        {
            for (int i = 0; i < stems.Count; i++)
                if (!string.IsNullOrEmpty(stems[i].id))
                    stems[i].id = stems[i].id.Trim().ToLowerInvariant();
        }
        // Warn if both full mix and stems provided
        if (HasFullMix && HasStems)
            Debug.LogWarning($"{name}: Both FullMix and Stems set. AudioService will prefer Stems.");
    }
#endif
}
