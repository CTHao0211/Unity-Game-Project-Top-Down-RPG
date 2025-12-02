using UnityEngine;

public class PlayerControllerMobile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    public Joystick moveJoystick;   // Joystick Pack (FixedJoystick)

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    // Cho kiếm đọc hướng di chuyển, giống PC
    public Vector2 Movement => movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        PlayerInput();
        AdjustPlayerFacingDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        if (movement.sqrMagnitude > 1f)
            movement = movement.normalized;

        if (myAnimator != null)
        {
            myAnimator.SetFloat("moveX", movement.x);
            myAnimator.SetFloat("moveY", movement.y);
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        if (movement.sqrMagnitude < 0.001f)
            return;

        if (movement.x < 0f)
        {
            mySpriteRender.flipX = true;   // nhìn sang trái
        }
        else if (movement.x > 0f)
        {
            mySpriteRender.flipX = false;  // nhìn sang phải
        }
    }
}
