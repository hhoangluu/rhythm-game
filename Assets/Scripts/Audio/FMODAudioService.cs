using UnityEngine;
using Doulingo.Core;

/// <summary>
/// Template for future FMOD implementation.
/// This shows how easy it will be to switch from Unity Audio to FMOD.
/// </summary>
public class FMODAudioService : MonoBehaviour, IAudioService
{
    [Header("FMOD Settings")]
    [SerializeField] private string fmodBankPath = "Banks/";
    [SerializeField] private string fmodMasterBus = "Bus:/Master";
    
    // FMOD-specific fields would go here
    // private FMOD.Studio.System fmodSystem;
    // private FMOD.Studio.Bank masterBank;
    // private FMOD.Studio.EventInstance[] stemInstances;

    #region IBeatSyncSource Implementation
    
    public double DspTime => 0.0; // TODO: Implement FMOD DSP time
    
    public double SongDspStartTime => 0.0; // TODO: Implement FMOD song start time
    
    public int SampleRate => 48000; // TODO: Get from FMOD system
    
    #endregion

    #region IMusicTransport Implementation
    
    public void LoadSong(SongDescriptor song)
    {
        // TODO: Load FMOD banks and events
        Debug.Log($"[FMODAudioService] Would load song: {song.songId} at {song.bpmBase} BPM");
    }
    
    public void Play(double dspStart = 0)
    {
        // TODO: Start FMOD events
        Debug.Log("[FMODAudioService] Would start FMOD playback");
    }
    
    public void Pause()
    {
        // TODO: Pause FMOD events
        Debug.Log("[FMODAudioService] Would pause FMOD playback");
    }
    
    public void Stop()
    {
        // TODO: Stop FMOD events
        Debug.Log("[FMODAudioService] Would stop FMOD playback");
    }
    
    public bool IsPlaying => false; // TODO: Check FMOD event states
    
    public double GetSongTimeSec() => 0.0; // TODO: Get from FMOD timeline
    
    #endregion

    #region IStemMixer Implementation
    
    public void SetStemVolume(string stemId, float linear01, float fadeMs = 60)
    {
        // TODO: Set FMOD bus volume
        Debug.Log($"[FMODAudioService] Would set {stemId} volume to {linear01:F2}");
    }
    
    public void Mute(string stemId, float fadeMs = 120)
    {
        // TODO: Mute FMOD bus
        Debug.Log($"[FMODAudioService] Would mute {stemId}");
    }
    
    public void Unmute(string stemId, float fadeMs = 120)
    {
        // TODO: Unmute FMOD bus
        Debug.Log($"[FMODAudioService] Would unmute {stemId}");
    }
    
    public void SetMasterVolume(float linear01, float fadeMs = 0)
    {
        // TODO: Set FMOD master bus volume
        Debug.Log($"[FMODAudioService] Would set master volume to {linear01:F2}");
    }
    
    #endregion

    #region IClickTrack Implementation
    
    public void EnableClick(bool on)
    {
        // TODO: Enable/disable FMOD click event
        Debug.Log($"[FMODAudioService] Would {(on ? "enable" : "disable")} click track");
    }
    
    public void SetClickSubdivision(int perBeat)
    {
        // TODO: Set FMOD click event parameter
        Debug.Log($"[FMODAudioService] Would set click subdivision to {perBeat}");
    }
    
    public void SetClickVolume(float linear01)
    {
        // TODO: Set FMOD click event volume
        Debug.Log($"[FMODAudioService] Would set click volume to {linear01:F2}");
    }
    
    #endregion

    #region ISfxService Implementation
    
    public void PlayClick()
    {
        // TODO: Play FMOD click sound
        Debug.Log("[FMODAudioService] Would play click sound");
    }
    
    public void PlayKey(float semitoneOffset = 0f)
    {
        // TODO: Play FMOD key sound with pitch
        Debug.Log($"[FMODAudioService] Would play key at {semitoneOffset:F1} semitones");
    }
    
    public void PlayOne(AudioClip clip, float vol01 = 1f)
    {
        // TODO: Play FMOD one-shot sound
        Debug.Log($"[FMODAudioService] Would play one-shot sound at volume {vol01:F2}");
    }
    
    #endregion

    private void Awake()
    {
        // TODO: Initialize FMOD system
        Debug.Log("[FMODAudioService] Would initialize FMOD system");
    }

    private void OnDestroy()
    {
        // TODO: Cleanup FMOD resources
        Debug.Log("[FMODAudioService] Would cleanup FMOD resources");
    }

    public void SetPitch(float ratio)
    {
        throw new System.NotImplementedException();
    }
}
