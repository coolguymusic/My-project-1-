using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioSource bounceAudioSource;
    public AudioSource dashAudioSource;

    private Rigidbody2D rb;
    private Animator animator;
    private TrailRenderer trail;

    private int airDashesRemaining;
    private bool isDashing;
    private bool isGrounded;
    private bool isResting;

    private float lastMoveDirection = 1f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
            trail.enabled = false;
    }

    void Start()
    {
        airDashesRemaining = maxAirDashes;
    }

    void Update()
    {
        float moveInput = 0f;

        if (isResting)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                isResting = false;
                animator.SetTrigger("launch");

                float direction = Keyboard.current.rightArrowKey.wasPressedThisFrame ? 1f : -1f;
                lastMoveDirection = direction;
                rb.linearVelocity = new Vector2(direction * moveSpeed, jumpForce);
            }
            return;
        }

        if (!isDashing)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveInput = -1f;
                lastMoveDirection = -1f;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveInput = 1f;
                lastMoveDirection = 1f;
            }

            if (moveInput != 0f)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = moveInput < 0f ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
                transform.localScale = localScale;
            }

            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isDashing && airDashesRemaining > 0 && !isGrounded)
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

        if (trail != null)
            trail.enabled = true;

        if (dashSound != null && dashAudioSource != null)
        {
            dashAudioSource.PlayOneShot(dashSound);
        }

        yield return new WaitForSeconds(0.2f);

        if (trail != null)
            trail.enabled = false;

        isDashing = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            ContactPoint2D[] contacts = collision.contacts;
            foreach (var contact in contacts)
            {
                if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    isGrounded = true;
                    animator.SetTrigger("land");

                    if (jumpSound != null && bounceAudioSource != null)
                    {
                        bounceAudioSource.PlayOneShot(jumpSound);
                    }

                    break;
                }
            }
        }
        else if (collision.collider.CompareTag("RestPlatform"))
        {
            ContactPoint2D[] contacts = collision.contacts;
            foreach (var contact in contacts)
            {
                if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
                {
                    rb.linearVelocity = Vector2.zero;
                    isGrounded = true;
                    isResting = true;
                    animator.SetTrigger("rest");
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
        }
    }
}
