using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODAudioManager : MonoBehaviour
{
    private EventInstance nonRestMusicEventInstance;
    private EventInstance restSFXEventInstance;
    private bool isResting = false;

    void Start()
    {
        // Initialize non-resting music event
        nonRestMusicEventInstance = RuntimeManager.CreateInstance("event:/Music/Scene 1");

        if (nonRestMusicEventInstance.isValid())
        {
            nonRestMusicEventInstance.start();
            Debug.Log("FMOD music event started");
        }
        else
        {
            Debug.LogError("Failed to create FMOD music event instance");
        }

        // Initialize the rest SFX event
        restSFXEventInstance = RuntimeManager.CreateInstance("event:/SFX/PlayerRest");
        restSFXEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        restSFXEventInstance.start();
        restSFXEventInstance.setParameterByName("IsResting", 1f); // Start in resting state
    }

    public void SetResting(bool isResting)
    {
        if (this.isResting == isResting) return;

        this.isResting = isResting;
        Debug.Log($"Setting resting state: {isResting}");

        if (nonRestMusicEventInstance.isValid())
        {
            nonRestMusicEventInstance.setParameterByName("IsResting", isResting ? 1f : 0f);
            Debug.Log(isResting ? "Music set to resting state." : "Music set to non-resting state.");
        }

        if (restSFXEventInstance.isValid())
        {
            restSFXEventInstance.setParameterByName("IsResting", isResting ? 1f : 0f);
            Debug.Log(isResting ? "Rest SFX faded in." : "Rest SFX faded out.");
        }
    }

    public void RestartNonRestMusic()
    {
        if (nonRestMusicEventInstance.isValid())
        {
            nonRestMusicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            nonRestMusicEventInstance.start();
            Debug.Log("Non-rest music restarted from the beginning.");
        }
        else
        {
            Debug.LogError("FMOD event instance is invalid!");
        }
    }

    void OnDestroy()
    {
        if (nonRestMusicEventInstance.isValid())
        {
            nonRestMusicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            nonRestMusicEventInstance.release();
            Debug.Log("FMOD music event stopped and released.");
        }

        if (restSFXEventInstance.isValid())
        {
            restSFXEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            restSFXEventInstance.release();
            Debug.Log("FMOD rest SFX event stopped and released.");
        }
    }
}