using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling }

    [SerializeField] private AudioSource jumpSoundEffect;

    private bool isJumping = false;
    private bool doubleTap = false;
    private float lastTapTime = 0f;
    private float doubleTapDelay = 0.2f; // Adjust as needed

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // For Android, check touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Assuming we only consider the first touch

            // Check if the touch is on the right or left half of the screen
            if (touch.position.x > Screen.width / 2)
            {
                dirX = 1f;
            }
            else
            {
                dirX = -1f;
            }

            // Check for double tap
            if (touch.tapCount == 2)
            {
                doubleTap = true;
            }
        }
        else
        {
            dirX = 0f;
            doubleTap = false;
        }

        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        // Jumping logic
        if (doubleTap && IsGrounded() && !isJumping)
        {
            Jump();
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private void Jump()
    {
        jumpSoundEffect.Play();
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
        Invoke("ResetJump", 0.1f); // Adjust delay if needed
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
