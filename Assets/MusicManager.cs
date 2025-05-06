using UnityEngine;
using UnityEngine.Audio;
using System.Collections; // Add this line

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip[] musicTracks;

    [Header("Music Routing")]
    public AudioMixer audioMixer; // Reference to the Audio Mixer
    public string musicVolumeParameter = "MusicVolume"; // Must match the parameter name in the mixer
    public AudioMixerGroup musicMixerGroup; // Audio Mixer Group for music routing

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public float crossfadeDuration = 2f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private bool isPlayingA = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup two AudioSources for crossfading
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        SetupSource(sourceA);
        SetupSource(sourceB);

        // Start with the first track if available
        if (musicTracks.Length > 0)
        {
            PlayMusic(musicTracks[0]);
        }
    }

    void SetupSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;

        if (musicMixerGroup != null)
        {
            source.outputAudioMixerGroup = musicMixerGroup;
        }
        else
        {
            Debug.LogWarning("MusicManager: No AudioMixerGroup assigned!");
        }
    }

    void Update()
    {
        // Sync volume for debugging/tuning
        AudioSource activeSource = isPlayingA ? sourceA : sourceB;
        if (activeSource.volume != musicVolume)
        {
            activeSource.volume = musicVolume;
        }

        SetMixerVolume(musicVolume); // Keep the mixer volume in sync with the inspector volume
    }

    public void SetVolume(float volume)
    {
        musicVolume = volume;
        sourceA.volume = volume;
        sourceB.volume = volume;

        SetMixerVolume(volume); // Sync with the Audio Mixer
    }

    public void SetMixerVolume(float volume)
    {
        if (audioMixer == null) return;

        // Convert linear to dB
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(musicVolumeParameter, dB); // Set volume in Audio Mixer
    }

    public void StopMusic()
    {
        sourceA.Stop();
        sourceB.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        AudioSource activeSource = isPlayingA ? sourceA : sourceB;
        activeSource.clip = clip;
        activeSource.volume = musicVolume;
        activeSource.Play();
    }

    public void CrossfadeTo(AudioClip newClip)
    {
        StartCoroutine(CrossfadeCoroutine(newClip));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
        AudioSource fromSource = isPlayingA ? sourceA : sourceB;
        AudioSource toSource = isPlayingA ? sourceB : sourceA;

        isPlayingA = !isPlayingA;

        toSource.clip = newClip;
        toSource.volume = 0f;
        toSource.Play();

        float time = 0f;

        while (time < crossfadeDuration)
        {
            float t = time / crossfadeDuration;
            fromSource.volume = Mathf.Lerp(musicVolume, 0f, t);
            toSource.volume = Mathf.Lerp(0f, musicVolume, t);
            time += Time.deltaTime;
            yield return null;
        }

        fromSource.Stop();
        toSource.volume = musicVolume;
    }
}
