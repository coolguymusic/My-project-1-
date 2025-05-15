using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump / Bounce")]
    public float jumpForce = 10f;

    [Header("Air Dash")]
    public float airDashForce = 15f;
    public int maxAirDashes = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private TrailRenderer trail;
    private FMODAudioManager audioManager;
    private PlayerInputHandler inputHandler;

    private int airDashesRemaining;
    private bool isDashing;
    private bool isGrounded;
    private bool isResting;
    private bool isExitingRest = false;
    
    private bool isLaunched = false;  // <-- New flag to track if the player is launched

    private float lastMoveDirection = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trail = GetComponent<TrailRenderer>();
        audioManager = FindAnyObjectByType<FMODAudioManager>();
        inputHandler = GetComponent<PlayerInputHandler>();

        if (trail != null)
            trail.enabled = false;
    }

    void Start()
    {
        airDashesRemaining = maxAirDashes;
    }

    void Update()
    {
        float moveInput = inputHandler.MoveInput.x;

        if (isResting)
        {
            if (moveInput != 0f)
            {
                isResting = false;
                animator.SetTrigger("launch");
                isExitingRest = true;

                lastMoveDirection = Mathf.Sign(moveInput);
                rb.linearVelocity = new Vector2(lastMoveDirection * moveSpeed, jumpForce);

                audioManager?.SetResting(false);
            }
            return;
        }

        if (!isDashing && !isLaunched)  // <-- Check isLaunched here
        {
            if (moveInput != 0f)
            {
                lastMoveDirection = Mathf.Sign(moveInput);
                Vector3 scale = transform.localScale;
                scale.x = lastMoveDirection < 0f ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // Handle air dash
        if (inputHandler.DashPressed && !isDashing && airDashesRemaining > 0 && !isGrounded)
        {
            StartCoroutine(PerformAirDash());
        }

        animator.SetFloat("verticalSpeed", rb.linearVelocity.y);
        animator.SetBool("isDescending", rb.linearVelocity.y < -0.1f && !isGrounded);

        if (isGrounded && rb.linearVelocity.y > 0.1f)
        {
            animator.SetTrigger("launch");
        }

        if (isGrounded)
        {
            airDashesRemaining = maxAirDashes;
        }
    }

    private System.Collections.IEnumerator PerformAirDash()
    {
        isDashing = true;
        airDashesRemaining--;

        float dashDir = lastMoveDirection;
        rb.linearVelocity = new Vector2(dashDir * airDashForce, 0f);
        animator.SetTrigger("dash");

        if (trail != null) trail.enabled = true;

        RuntimeManager.PlayOneShot("event:/SFX/PlayerDash", transform.position);

        yield return new WaitForSeconds(0.2f);

        if (trail != null) trail.enabled = false;

        isDashing = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    isGrounded = true;
                    animator.SetTrigger("land");

                    RuntimeManager.PlayOneShot("event:/SFX/PlayerJump", transform.position);
                    audioManager?.SetResting(false);
                    break;
                }
            }
        }
        else if (collision.collider.CompareTag("RestPlatform"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
                {
                    if (!isResting)
                    {
                        rb.linearVelocity = Vector2.zero;
                        isGrounded = true;
                        isResting = true;
                        animator.SetTrigger("rest");
                        audioManager?.SetResting(true);
                    }
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("RestPlatform"))
        {
            isGrounded = false;

            if (isExitingRest && isGrounded)
            {
                audioManager?.SetResting(false);
                isExitingRest = false;
            }
        }
    }

    // Add Launch Method:
   public void Launch()
{
    isLaunched = true;
    Invoke(nameof(ResetLaunch), 0.2f); // Prevents override for a short time
}

private void ResetLaunch()
{
    isLaunched = false;
}
}
