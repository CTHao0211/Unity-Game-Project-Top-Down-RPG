using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCombined : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    public Joystick moveJoystick; // Joystick cho mobile

    private PlayerControls playerControls; // Input System cho PC
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    // Cho script khác đọc hướng di chuyển
    public Vector2 Movement => movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
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
        Vector2 pcInput = playerControls.Movement.Move.ReadValue<Vector2>();
        Vector2 joystickInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        // Chuẩn hóa joystick nếu quá lớn
        if (joystickInput.sqrMagnitude > 1f)
            joystickInput = joystickInput.normalized;

        // Ưu tiên AWSD/PC
        if (pcInput.sqrMagnitude > 0.01f)
        {
            movement = pcInput;
        }
        else if (joystickInput.sqrMagnitude > 0.01f)
        {
            movement = joystickInput;
        }
        else
        {
            movement = Vector2.zero;
        }

        // Giữ nguyên Animator và flip theo AWSD/joystick
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
            mySpriteRender.flipX = true;
        else if (movement.x > 0f)
            mySpriteRender.flipX = false;
    }
}
