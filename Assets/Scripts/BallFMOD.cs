using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BallFMOD : MonoBehaviour
{
    public EventReference bounceSound;
    public float maxDistance = 10f; // Max distance for volume fade

    private EventInstance bounceInstance;
    private Transform playerTransform;

    private void Start()
    {
        // Find player in scene (assumes tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        // Create the event instance but don't start it yet
        bounceInstance = RuntimeManager.CreateInstance(bounceSound);
        bounceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calculate distance between ball and player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate volume based on distance and maxDistance
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));

        bounceInstance.setVolume(volume);
        bounceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    public void PlayBounceSound()
    {
        // Stop if already playing, then start fresh
        bounceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bounceInstance.start();
    }

    private void OnDestroy()
    {
        bounceInstance.release();
    }
}
