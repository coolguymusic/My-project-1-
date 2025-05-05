using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string sceneToLoad = "Scene2"; // Replace with your actual scene name

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (SceneTransitionData.Instance != null && rb != null)
            {
                SceneTransitionData.Instance.playerPosition = other.transform.position;
                SceneTransitionData.Instance.playerVelocity = rb.linearVelocity;
                SceneTransitionData.Instance.entryDirection = "up"; // jumping upward
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
