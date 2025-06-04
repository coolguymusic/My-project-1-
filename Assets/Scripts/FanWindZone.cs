using UnityEngine;
using System.Collections;

public class FanWindZone : MonoBehaviour
{
    public Vector2 pushDirection = Vector2.right;
    public float pushForce = 20f;
    public float windOffDelay = 2f;

    private bool windActive = false;
    private Coroutine windOffCoroutine;

    public void EnableWind(bool enable)
    {
        if (enable)
        {
            // If wind turned on, stop any wind-off countdown
            if (windOffCoroutine != null)
            {
                StopCoroutine(windOffCoroutine);
                windOffCoroutine = null;
            }
            windActive = true;
        }
        else
        {
            // Start coroutine to turn wind off after delay
            if (windOffCoroutine == null)
            {
                windOffCoroutine = StartCoroutine(WindOffCountdown());
            }
        }
    }

    private IEnumerator WindOffCountdown()
    {
        yield return new WaitForSeconds(windOffDelay);
        windActive = false;
        windOffCoroutine = null;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!windActive) return;

        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(pushDirection.normalized * pushForce, ForceMode2D.Force);
            }
        }
    }
}
