using UnityEngine;

public class SceneTransitionData : MonoBehaviour
{
    public static SceneTransitionData Instance;

    public Vector2 playerPosition;
    public Vector2 playerVelocity;
    public string entryDirection;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
