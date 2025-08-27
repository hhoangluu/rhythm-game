using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Individual Piano Key Clips")]
    [SerializeField] private AudioClip[] pianoKeyClips; // Individual clips for each key
    
    [Header("Piano Key Audio Sources")]
    [SerializeField] private int numberOfKeys = 8; // A-K keys
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 0.3f; // How long to fade out
    [SerializeField] private float fadeOutStartVolume = 1f; // Volume when key is pressed
    
    private AudioSource[] keyAudioSources;
    private Dictionary<int, AudioSource> keySourceMap;
    private List<int> playingKeys; // Simple list of keys currently playing

    private void Start()
    {
        InitializeKeyAudioSources();
    }

    private void InitializeKeyAudioSources()
    {
        // Validate clips array
        if (pianoKeyClips == null || pianoKeyClips.Length < numberOfKeys)
        {
            Debug.LogError($"[SfxPlayer] Not enough piano key clips! Need {numberOfKeys} clips, but got {(pianoKeyClips?.Length ?? 0)}");
            return;
        }
        
        keyAudioSources = new AudioSource[numberOfKeys];
        keySourceMap = new Dictionary<int, AudioSource>();
        playingKeys = new List<int>();

        for (int i = 0; i < numberOfKeys; i++)
        {
            // Create individual AudioSource for each key
            GameObject keyObj = new GameObject($"KeyAudio_{i}");
            keyObj.transform.SetParent(transform);
            
            AudioSource keySource = keyObj.AddComponent<AudioSource>();
            keySource.clip = pianoKeyClips[i]; // Assign the specific clip
            keySource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            keySource.volume = fadeOutStartVolume;
            keySource.playOnAwake = false;
            keySource.loop = false; // Don't loop
            
            keyAudioSources[i] = keySource;
            keySourceMap[i] = keySource;
            
            Debug.Log($"[SfxPlayer] Created AudioSource for key {i} with clip: {pianoKeyClips[i]?.name ?? "null"}");
        }
    }

    /// <summary>
    /// Set piano key clips and reinitialize audio sources
    /// </summary>
    public void SetPianoKeyClips(AudioClip[] newClips)
    {
        if (newClips == null || newClips.Length < numberOfKeys)
        {
            Debug.LogError($"[SfxPlayer] Invalid clips array! Need {numberOfKeys} clips, but got {(newClips?.Length ?? 0)}");
            return;
        }
        
        pianoKeyClips = newClips;
        InitializeKeyAudioSources();
        Debug.Log($"[SfxPlayer] Piano key clips updated and audio sources reinitialized");
    }

    public void PlayKey(int keyIndex)
    {
        // Play specific piano key with its own AudioSource
        if (keySourceMap.TryGetValue(keyIndex, out AudioSource keySource))
        {
            // Add key to playing list and set volume to max
            if (!playingKeys.Contains(keyIndex))
            {
                playingKeys.Add(keyIndex);
            }
            
            // Reset volume to full and restart sound naturally
            keySource.volume = fadeOutStartVolume;
            keySource.Stop(); // Stop any currently playing sound
            keySource.Play();
            
            Debug.Log($"[SfxPlayer] Playing key {keyIndex}");
        }
        else
        {
            Debug.LogWarning($"[SfxPlayer] No AudioSource found for key {keyIndex}");
        }
    }

    public void StopKey(int keyIndex)
    {
        // Stop specific piano key with smooth fade out
        if (keySourceMap.TryGetValue(keyIndex, out AudioSource keySource))
        {
            // Remove key from playing list
            playingKeys.Remove(keyIndex);
            
            // Start fade out
            StartCoroutine(FadeOutKey(keyIndex, keySource));
            
            Debug.Log($"[SfxPlayer] Stopping key {keyIndex} with fade out");
        }
        else
        {
            Debug.LogWarning($"[SfxPlayer] No AudioSource found for key {keyIndex}");
        }
    }

    private IEnumerator FadeOutKey(int keyIndex, AudioSource keySource)
    {
        float startVolume = keySource.volume;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            // Check if key is playing again - if so, stop fade and break
            if (playingKeys.Contains(keyIndex))
            {
                keySource.volume = fadeOutStartVolume;
                Debug.Log($"[SfxPlayer] Key {keyIndex} is playing again, stopping fade");
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeOutDuration;
            
            // Smooth fade out curve
            keySource.volume = Mathf.Lerp(startVolume, 0f, progress);
            
            yield return null;
        }
        
        // Ensure volume is exactly 0
        keySource.volume = 0f;
        keySource.Stop();
        
        Debug.Log($"[SfxPlayer] Key {keyIndex} fade out complete");
    }
}
