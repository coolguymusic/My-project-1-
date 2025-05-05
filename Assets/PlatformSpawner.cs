using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public int initialPlatforms = 10;
    public float xRange = 2.5f;
    public float minYGap = 1.5f;
    public float maxYGap = 2.5f;

    private float lastY = -4.8f; // ✅ Start closer to the starting platform

    void Start()
    {
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    public void SpawnPlatform()
    {
        float yGap = Random.Range(minYGap, maxYGap);
        lastY += yGap;
        float x = Random.Range(-xRange, xRange);

        Vector3 spawnPos = new Vector3(x, lastY, 0);
        Instantiate(platformPrefab, spawnPos, Quaternion.identity);
    }
}
