using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Load all scenes except the active one
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            if (i != activeSceneIndex)
            {
                SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
            }
        }
    }
}
