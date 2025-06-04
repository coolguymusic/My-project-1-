using UnityEngine;

public class FanTrigger : MonoBehaviour
{
    public FanWindZone windZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            windZone.EnableWind(true);  // Start wind
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            windZone.EnableWind(false); // Delay turning wind off
        }
    }
}
