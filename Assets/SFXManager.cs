using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("SFX Routing")]
    public AudioMixer audioMixer; // 👈 Assign the whole mixer
    public string sfxVolumeParameter = "SFXVolume"; // 👈 Must match exposed parameter name
    public AudioMixerGroup sfxMixerGroup;

    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("SFX AudioSources (Assign from Player)")]
    public AudioSource sfxSourceA;
    public AudioSource sfxSourceB;

    private bool useA = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupSource(sfxSourceA);
        SetupSource(sfxSourceB);
        SetMixerVolume(sfxVolume); // initialize mixer level
    }

    void SetupSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogWarning("SFXManager: One or more SFX AudioSources not assigned.");
            return;
        }

        source.loop = false;
        source.playOnAwake = false;
        source.volume = sfxVolume;

        if (sfxMixerGroup != null)
        {
            source.outputAudioMixerGroup = sfxMixerGroup;
        }
        else
        {
            Debug.LogWarning("SFXManager: No AudioMixerGroup assigned.");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource current = useA ? sfxSourceA : sfxSourceB;
        current.clip = clip;
        current.volume = sfxVolume;
        current.Play();

        useA = !useA;
    }

    public void SetVolume(float volume)
    {
        sfxVolume = volume;

        if (sfxSourceA) sfxSourceA.volume = volume;
        if (sfxSourceB) sfxSourceB.volume = volume;

        SetMixerVolume(volume);
    }

    public void SetMixerVolume(float volume)
    {
        if (audioMixer == null) return;

        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(sfxVolumeParameter, dB);
    }
}
