using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCombined : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    public CanvasGroup dashButtonCanvasGroup;
    public Joystick moveJoystick; // Joystick cho mobile
    public static PlayerControllerCombined instance;

    private PlayerControls playerControls; // Input System cho PC
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    // Cho script khác đọc hướng di chuyển
    public Vector2 Movement => movement;
    private float startingMoveSpeed;
    private bool isDashing = false;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();

        playerControls = new PlayerControls();
    }
    private void Start() {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;
    }
    public void OnDashButton()
    {
        Dash();
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
    private void Dash() {
        if (!isDashing) {
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

private IEnumerator EndDashRoutine() {
    float dashTime = .2f;
    float dashCooldown = 3f; // 3 GIÂY COOLDOWN

    yield return new WaitForSeconds(dashTime);

    moveSpeed = startingMoveSpeed;  
    myTrailRenderer.emitting = false;

    yield return StartCoroutine(DashCooldownRoutine(dashCooldown));

    isDashing = false;
}


    private IEnumerator DashCooldownRoutine(float cooldownTime)
    {
        if (dashButtonCanvasGroup)
        {
            // Khi bắt đầu cooldown → mờ đi
            dashButtonCanvasGroup.alpha = 0.3f;
            dashButtonCanvasGroup.interactable = false;
        }

        float elapsed = 0f;

        // Làm sáng dần trong suốt cooldown
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / cooldownTime;

            if (dashButtonCanvasGroup)
                dashButtonCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, t);

            yield return null;
        }

        // Cooldown xong → sáng hoàn toàn + có thể nhấn lại
        if (dashButtonCanvasGroup)
        {
            dashButtonCanvasGroup.alpha = 1f;
            dashButtonCanvasGroup.interactable = true;
        }
    }

}
