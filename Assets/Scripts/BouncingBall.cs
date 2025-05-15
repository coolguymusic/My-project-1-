using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncingBall : MonoBehaviour
{
    public float bounceForce = 10f;
    public float launchForce = 20f;
    public float xLaunchMultiplier = 2f; // Adjustable in Inspector
    public float yLaunchMultiplier = 1f; // Optional: if you ever want to tweak Y too

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Set collision detection to Continuous for better high-speed collision detection
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 hitDirection = (other.transform.position - transform.position).normalized;

                Vector2 launchDirection = new Vector2(
                    hitDirection.x * launchForce * xLaunchMultiplier,
                    hitDirection.y * launchForce * yLaunchMultiplier
                );

                playerRb.linearVelocity = launchDirection;

                // Call Launch() method in PlayerController to prevent horizontal input while launched
                other.GetComponent<PlayerController>()?.Launch();  // Corrected: no arguments here
            }
        }
    }
}
