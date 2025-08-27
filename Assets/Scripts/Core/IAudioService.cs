using UnityEngine;

namespace Doulingo.Core
{
    public interface IBeatSyncSource
    {
        double DspTime { get; }
        double SongDspStartTime { get; }
        int SampleRate { get; }
    }

    public interface IMusicTransport
    {
        void LoadSong(SongDescriptor song);
        void Play(double dspStart = 0);
        void Pause();
        void Stop();
        void SetPitch(float ratio);
        bool IsPlaying { get; }
        double GetSongTimeSec();
    }

    public interface IStemMixer
    {
        void SetStemVolume(string stemId, float linear01, float fadeMs = 60);
        void Mute(string stemId, float fadeMs = 120);
        void Unmute(string stemId, float fadeMs = 120);
        void SetMasterVolume(float linear01, float fadeMs = 0);
    }

    public interface IClickTrack
    {
        void EnableClick(bool on);
        void SetClickSubdivision(int perBeat);
        void SetClickVolume(float linear01);
    }

    public interface IAudioService : IBeatSyncSource, IMusicTransport, IStemMixer, IClickTrack
    {
        // main entry point
    }
}
