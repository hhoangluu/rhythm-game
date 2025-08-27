using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Doulingo.Core;

public class UnityAudioService : MonoBehaviour, IAudioService
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sources")]
    [SerializeField] private AudioSource fullMixSrc;
    [SerializeField] private AudioSource stemVocal;
    [SerializeField] private AudioSource stemDrums;
    [SerializeField] private AudioSource stemBass;
    [SerializeField] private AudioSource stemOther;
    [SerializeField] private AudioSource stemBeat; // metronome clicks

    [Header("Click Track")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private float scheduleAheadSec = 0.1f;

    private Dictionary<string, AudioSource> stems = new();
    private SongDescriptor song;
    private double dspStart;
    private int sampleRate;
    private bool clickEnabled;
    private double nextClickDsp;
    private double secondsPerBeat;
    private int clicksPerBeat = 1;

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        stems["vocal"] = stemVocal;
        stems["drums"] = stemDrums;
        stems["bass"] = stemBass;
        stems["other"] = stemOther;
        stems["beat"]  = stemBeat;
    }

    // === IMusicTransport ===
    public void LoadSong(SongDescriptor s)
    {
        song = s;
        fullMixSrc.clip = null;
        foreach (var kv in stems) if (kv.Value) kv.Value.clip = null;

        if (s.stems != null && s.stems.Count > 0)
        {
            foreach (var e in s.stems)
                if (stems.TryGetValue(e.id, out var src) && src) src.clip = e.clip;
        }
        else
        {
            fullMixSrc.clip = s.fullMix;
        }
    }

    public void Play(double dspStartTime = 0)
    {
        dspStart = dspStartTime > 0 ? dspStartTime : AudioSettings.dspTime + 0.05;
        
        // Schedule all audio sources to start at the same DSP time
        if (fullMixSrc.clip) fullMixSrc.PlayScheduled(dspStart);
        foreach (var kv in stems) if (kv.Value && kv.Value.clip) kv.Value.PlayScheduled(dspStart);

        // click track setup
        secondsPerBeat = 60.0 / song.bpmBase;
        nextClickDsp = dspStart + song.leadInSec;
        
        Debug.Log($"[UnityAudioService] Scheduled song to play at DSP time: {dspStart:F3}");
    }

    // === IMusicTransport SetPitch ===
    public void SetPitch(float ratio)
    {
        // Adjust pitch to change tempo (ratio = currentBpm / baseBpm)
        float pitch = Mathf.Clamp(ratio, 0.5f, 2.0f);
        
        // Change pitch for music stems only (NOT the click track)
        if (fullMixSrc.clip) fullMixSrc.pitch = pitch;
        foreach (var kv in stems) 
        {
            // Skip the click track - it should maintain original pitch
            if (kv.Value && kv.Value.clip && kv.Value != stemBeat) 
            {
                kv.Value.pitch = pitch;
            }
        }
        
        // CRITICAL FIX: Recalculate click timing to match new BPM
        if (clickEnabled && song != null)
        {
            // Calculate new BPM from ratio
            float newBPM = song.bpmBase * ratio;
            
            // Update secondsPerBeat to match new BPM (this is used in Update())
            secondsPerBeat = 60.0f / newBPM;
            
            // Recalculate next click DSP time based on new timing
            RecalculateClickTiming();
            
            Debug.Log($"[UnityAudioService] Click timing updated: new BPM {newBPM:F1}, secondsPerBeat: {secondsPerBeat:F3}s, nextClickDsp: {nextClickDsp:F3} (click pitch unchanged)");
        }
        
    }

    /// <summary>
    /// Recalculates click timing to ensure consistency between Update() and SetPitch
    /// </summary>
    private void RecalculateClickTiming()
    {
        if (!clickEnabled || song == null) return;
        
        double currentDspTime = AudioSettings.dspTime;
        
        // CRITICAL FIX: Use the tracked last beat position for proper BPM change handling
        double lastBeatDsp;
        
        if (lastBeatDspTime > 0)
        {
            // Use the tracked last beat position (this preserves the original beat structure)
            lastBeatDsp = lastBeatDspTime;
        }
        else
        {
            // Fallback: calculate based on current time
            double timeSinceStart = currentDspTime - dspStart;
            double beatsPassed = Mathf.Floor((float)(timeSinceStart / secondsPerBeat));
            lastBeatDsp = dspStart + (beatsPassed * secondsPerBeat);
        }
        
        // Calculate remaining time to next beat using speed ratio
        double timeSinceLastBeat = currentDspTime - lastBeatDsp;
        double oldSecondsPerBeat = 60.0 / song.bpmBase; // Original BPM timing
        
        // CRITICAL FIX: Calculate remaining time to next beat and apply BPM ratio
        // New remaining time = Original remaining time / BPM ratio
        
        // Calculate the original next beat position
        double originalNextBeatDsp = lastBeatDsp + oldSecondsPerBeat;
        
        // Calculate remaining time to the original next beat
        double originalRemainingTime = originalNextBeatDsp - currentDspTime;
        
        // Calculate BPM ratio (old/new)
        double bpmRatio = oldSecondsPerBeat / secondsPerBeat;
        
        // Apply BPM ratio to get new remaining time
        double newRemainingTime = originalRemainingTime / bpmRatio;
        
        // Calculate the new next beat position
        double nextBeatDsp = currentDspTime + newRemainingTime;
        
        nextClickDsp = nextBeatDsp;
        
        Debug.Log($"[UnityAudioService] Click timing recalculated: last beat at {lastBeatDsp:F3}, original remaining: {originalRemainingTime:F3}s, BPM ratio: {bpmRatio:F3}, new remaining: {newRemainingTime:F3}s, next click at {nextBeatDsp:F3} (current DSP: {currentDspTime:F3}, BPM: {60.0/secondsPerBeat:F1})");
    }

    public void Pause()
    {
        if (fullMixSrc.clip) fullMixSrc.Pause();
        foreach (var kv in stems) if (kv.Value && kv.Value.clip) kv.Value.Pause();
    }

    public void Stop()
    {
        if (fullMixSrc.clip) fullMixSrc.Stop();
        foreach (var kv in stems) if (kv.Value && kv.Value.clip) kv.Value.Stop();
    }

    public bool IsPlaying =>
        (fullMixSrc && fullMixSrc.isPlaying) || AnyStemPlaying();

    public double GetSongTimeSec() =>
        Mathf.Max(0f, (float)(AudioSettings.dspTime - dspStart));

    private bool AnyStemPlaying()
    {
        foreach (var kv in stems) if (kv.Value && kv.Value.isPlaying) return true;
        return false;
    }

    // === IStemMixer ===
    public void Mute(string stemId, float fadeMs = 120) => SetStemVolume(stemId, 0f, fadeMs);
    public void Unmute(string stemId, float fadeMs = 120) => SetStemVolume(stemId, 1f, fadeMs);

    public void SetStemVolume(string stemId, float v, float fadeMs = 60)
    {
        if (!stems.TryGetValue(stemId, out var src) || !src) return;
        StartCoroutine(FadeVolume(src, v, fadeMs / 1000f));
    }

    public void SetMasterVolume(float v, float fadeMs = 0)
    {
        if (!mixer) return;
        float db = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(v));
        mixer.SetFloat("MasterVol", db);
    }

    private System.Collections.IEnumerator FadeVolume(AudioSource src, float target, float dur)
    {
        float t = 0f, start = src.volume;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        src.volume = target;
    }

    // === IClickTrack ===
    public void EnableClick(bool on) { clickEnabled = on; }
    public void SetClickSubdivision(int perBeat) { clicksPerBeat = Mathf.Max(1, perBeat); }
    public void SetClickVolume(float v) { if (stemBeat) stemBeat.volume = Mathf.Clamp01(v); }

    // Track the last beat position for BPM change handling
    private double lastBeatDspTime = 0;

    void Update()
    {
        if (!clickEnabled || !stemBeat || clickClip == null) return;

        double now = AudioSettings.dspTime;
        double secPerClick = secondsPerBeat / clicksPerBeat;

        // Only schedule clicks if we haven't already scheduled the next one
        // This prevents conflicts with RecalculateClickTiming
        if (nextClickDsp <= now + scheduleAheadSec)
        {
           
            
            // Schedule the next click
            stemBeat.clip = clickClip;
            stemBeat.PlayScheduled(nextClickDsp);
            lastBeatDspTime = nextClickDsp;

            nextClickDsp += secPerClick;
            
            Debug.Log($"[UnityAudioService] Click scheduled at DSP {nextClickDsp - secPerClick:F3}, next click at {nextClickDsp:F3} current time {now:F3}");
        }
    }

    // === IBeatSyncSource ===
    public double DspTime => AudioSettings.dspTime;
    public double SongDspStartTime => dspStart;
    public int SampleRate => sampleRate;
}
