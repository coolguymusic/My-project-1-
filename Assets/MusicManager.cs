using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f; // You can set this in the Inspector

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume; // Use inspector value
        audioSource.Play();
    }

    void Update()
    {
        // This keeps the volume in sync with the inspector in real time
        if (audioSource.volume != musicVolume)
        {
            audioSource.volume = musicVolume;
        }
    }

    public void SetVolume(float volume)
    {
        musicVolume = volume;
        audioSource.volume = volume;
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
