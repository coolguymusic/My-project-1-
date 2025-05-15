using UnityEngine;

[System.Serializable]
public struct CameraZone
{
    public float startY;             // Lower bound of the zone
    public float endY;               // Upper bound of the zone
    public bool isFreezeZone;        // Whether camera freezes in this zone
    public Vector3 freezePosition;   // Position to hold camera at
    public float freezeSmoothSpeed;  // Speed when freezing
}

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public float smoothSpeed = 5f;           // Normal smooth speed for following player
    public float longFallSmoothSpeed = 10f;  // Smooth speed when falling fast
    public float yOffset = 1f;
    public float minYPosition = 0f;
    public float hysteresisBuffer = 2f;      // Prevents camera jitter near zone edges
    public float longFallThreshold = -15f;   // Speed threshold beyond which freeze zones are ignored

    public CameraZone[] cameraZones;

    private float targetY;
    private bool shouldFollow = true;
    private CameraZone? currentZone = null;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (Player != null)
            targetY = Player.position.y;
    }

    void LateUpdate()
    {
        if (Player == null || cameraZones.Length == 0) return;

        float playerY = Player.position.y + yOffset;
        float verticalSpeed = Player.GetComponent<Rigidbody2D>()?.linearVelocity.y ?? 0f;
        bool bypassFreeze = verticalSpeed < longFallThreshold;

        // Hysteresis: Only switch zones if outside buffer
        foreach (var zone in cameraZones)
        {
            if (playerY > zone.startY - hysteresisBuffer && playerY < zone.endY + hysteresisBuffer)
            {
                // Skip freeze zone if falling fast
                if (zone.isFreezeZone && !bypassFreeze)
                {
                    shouldFollow = false;
                    Vector3 target = zone.freezePosition;
                    transform.position = Vector3.Lerp(transform.position, target, zone.freezeSmoothSpeed * Time.deltaTime);
                    currentZone = zone;
                    return;
                }
                else if (!zone.isFreezeZone)
                {
                    shouldFollow = true;
                    currentZone = zone;
                    break;
                }
            }
        }

        // If the camera should follow, update target position
        if (shouldFollow)
        {
            targetY = Mathf.Max(playerY, minYPosition);
            float currentSmoothSpeed = bypassFreeze ? longFallSmoothSpeed : smoothSpeed; // Use long fall speed if bypassing freeze zones
            Vector3 newPos = new Vector3(0f, targetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, currentSmoothSpeed * Time.deltaTime);
        }
    }

    public void StopFollowing() => shouldFollow = false;
    public void ResumeFollowing() => shouldFollow = true;
}
