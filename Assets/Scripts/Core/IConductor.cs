using UnityEngine;
using System;

namespace Doulingo.Core
{
    public interface IConductor
    {
        float CurrentBPM { get; }
        float SongPositionInBeats { get; }
        float SongPositionSec { get; }
        float BeatDuration { get; }
        bool IsPlaying { get; }

        event Action<float> OnBPMChanged;
        event Action OnSongStarted;
        event Action OnSongEnded;

        void Initialize(float initialBPM, float songStartDelay, float totalNotes, IBeatSyncSource beatSyncSource);
        void StartSong();
        void StopSong();
        void ChangeBPM(float newBPM);
        void UpdateSongPosition(float deltaTime);
    }
}
