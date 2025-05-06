using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Index of the music track to play from MusicManager's track list.")]
    public int musicTrackIndex = 0;

    [Tooltip("Only trigger once?")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if (other.CompareTag("Player"))
        {
            AudioClip clipToPlay = GetClipByIndex(musicTrackIndex);
            if (clipToPlay != null)
            {
                MusicManager.Instance.CrossfadeTo(clipToPlay);
                hasTriggered = true;
            }
            else
            {
                Debug.LogWarning("TriggerZone: Invalid musicTrackIndex or clip is missing.");
            }
        }
    }

    private AudioClip GetClipByIndex(int index)
    {
        if (MusicManager.Instance == null || MusicManager.Instance.musicTracks == null)
            return null;

        if (index >= 0 && index < MusicManager.Instance.musicTracks.Length)
            return MusicManager.Instance.musicTracks[index];

        return null;
    }
}
