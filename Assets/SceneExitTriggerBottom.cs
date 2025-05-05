using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitTriggerBottom : MonoBehaviour
{
    public string targetSceneName = "Scene1"; // Set this to your Scene1's name

    private bool isTransitioning = false; // Flag to prevent repeated transitions

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player and prevent re-triggering the transition
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true; // Set flag to prevent looping

            // Debugging: Output player entering trigger
            Debug.Log("Player entered Scene Exit Trigger in Scene 2.");

            // Store player’s position and velocity for Scene 1
            SceneTransitionData.Instance.playerPosition = other.transform.position;
            SceneTransitionData.Instance.playerVelocity = other.GetComponent<Rigidbody2D>().linearVelocity;

            // Indicate the player is coming from below in Scene 1
            SceneTransitionData.Instance.entryDirection = "down";

            // Load Scene 1 after storing the data
            SceneManager.LoadScene(targetSceneName);

            // Debugging: Output transition data
            Debug.Log("Player position and velocity saved for transition.");
        }
    }
}
