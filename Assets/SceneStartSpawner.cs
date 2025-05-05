using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStartSpawner : MonoBehaviour
{
    public Transform entryFromBelow; // Assign your SceneEntryFromBelow object here
    public float verticalOffset = 1f; // Adjust this to position the player just below the start of Scene 2

    private CameraFollow cameraFollowScript;
    private bool hasSpawnedPlayer = false; // Prevent multiple spawn attempts

    private void Start()
    {
        // Prevent loading this scene repeatedly by checking if player has been spawned
        if (hasSpawnedPlayer)
        {
            return;
        }

        // Find the CameraFollow script dynamically using the newer method
        cameraFollowScript = Object.FindFirstObjectByType<CameraFollow>();

        if (SceneTransitionData.Instance != null && SceneTransitionData.Instance.entryDirection == "up")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

                // Set X position from previous scene and Y position just below the entry point in Scene 2
                Vector3 newPos = entryFromBelow.position;
                newPos.x = SceneTransitionData.Instance.playerPosition.x;
                newPos.y = entryFromBelow.position.y - verticalOffset; // Position the player just below the start of Scene 2
                player.transform.position = newPos;

                // Debugging: Output player position
                Debug.Log("Player spawn position: " + player.transform.position);

                // Restore vertical velocity (to maintain momentum from Scene 1)
                if (rb != null)
                {
                    rb.linearVelocity = SceneTransitionData.Instance.playerVelocity;

                    // Debugging: Output player velocity
                    Debug.Log("Player velocity: " + rb.linearVelocity);
                }

             

                // Mark as spawned to prevent repeating
                hasSpawnedPlayer = true;
            }
        }
    }
}


