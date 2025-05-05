using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;
    public float parallaxFactor = 0.5f;

    private Vector3 previousPlayerPosition;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        previousPlayerPosition = player.position;
    }

    void LateUpdate()
    {
        Vector3 delta = player.position - previousPlayerPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        previousPlayerPosition = player.position;
    }
}
