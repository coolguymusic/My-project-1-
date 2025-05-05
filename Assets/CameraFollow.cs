using UnityEngine;

[System.Serializable]
public struct CameraZone
{
    public float startY;        // The starting Y position for the zone
    public float endY;          // The ending Y position for the zone
    public bool isFreezeZone;   // Whether this is a freeze zone
    public Vector3 freezePosition; // The position to move the camera to when freezing
    public float freezeSmoothSpeed; // The smooth speed to use when freezing in this zone
}

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public float smoothSpeed = 5f; // Default smooth speed when following player
    public float yOffset = 1f;
    public float minYPosition = 0f;  // Minimum Y value for camera

    public CameraZone[] cameraZones;

    private float targetY;
    private bool shouldFollow = true;

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

        // Loop through the zones to check if the camera should stop or resume following
        foreach (var zone in cameraZones)
        {
            // If in a freeze zone, stop the camera from following and smoothly move it to freeze position
            if (playerY >= zone.startY && playerY <= zone.endY && zone.isFreezeZone)
            {
                shouldFollow = false;
                // Smoothly move to the freeze position using zone's specific smooth speed
                Vector3 targetPosition = zone.freezePosition;
                transform.position = Vector3.Lerp(transform.position, targetPosition, zone.freezeSmoothSpeed * Time.deltaTime);
                Debug.Log($"Camera stopped at freeze zone. Player Y: {playerY}, Camera moved to: {targetPosition}");
                return;  // Exit the loop early since we've already frozen the camera
            }

            // If in a follow zone, resume following
            if (playerY >= zone.startY && playerY <= zone.endY && !zone.isFreezeZone)
            {
                shouldFollow = true;
                Debug.Log($"Camera following in follow zone. Player Y: {playerY}");
                break;  // Exit the loop once we've resumed following
            }
        }

        // If following, update the target Y position
        if (shouldFollow)
        {
            targetY = playerY;
            targetY = Mathf.Max(targetY, minYPosition); // Ensures camera never goes below minYPosition
            Vector3 newPos = new Vector3(0f, targetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
            Debug.Log($"Camera following. Target Y: {targetY}, Player Y: {playerY}");
        }
    }

    public void StopFollowing() => shouldFollow = false;
    public void ResumeFollowing() => shouldFollow = true;
}
