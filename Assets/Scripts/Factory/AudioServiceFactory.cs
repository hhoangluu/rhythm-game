using UnityEngine;
using Doulingo.Core;
using Doulingo.Audio;

namespace Doulingo.Factory
{
    public enum AudioServiceType
    {
        UnityAudioService,
        FMODAudioService
    }

    public class AudioServiceFactory : MonoBehaviour
    {
        [Header("Audio Service Prefabs")]
        [SerializeField] private UnityAudioService unityAudioServicePrefab;
        [SerializeField] private FMODAudioService fmodAudioServicePrefab;

        public IAudioService GetAudioService(AudioServiceType type)
        {
            switch (type)
            {
                case AudioServiceType.UnityAudioService:
                    if (unityAudioServicePrefab != null)
                    {
                        return Instantiate(unityAudioServicePrefab);
                    }
                    else
                    {
                        Debug.LogError("[AudioServiceFactory] UnityAudioService prefab not assigned!");
                        return null;
                    }
                
                case AudioServiceType.FMODAudioService:
                    if (fmodAudioServicePrefab != null)
                    {
                        return Instantiate(fmodAudioServicePrefab);
                    }
                    else
                    {
                        Debug.LogError("[AudioServiceFactory] FMODAudioService prefab not assigned!");
                        return null;
                    }
                
                default:
                    Debug.LogError($"[AudioServiceFactory] Unknown AudioService type: {type}");
                    return null;
            }
        }
    }
}
