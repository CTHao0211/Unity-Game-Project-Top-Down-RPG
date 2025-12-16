using System.Collections;
using UnityEngine;

public class PlayerControllerCombined : Singleton<PlayerControllerCombined>
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    public CanvasGroup dashButtonCanvasGroup;
    public Joystick moveJoystick; // Mobile joystick

    private PlayerControls playerControls;
    private Vector2 movement;
    [SerializeField] private Rigidbody2D rb;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    private Vector2 lastMovement = Vector2.down;
    private float startingMoveSpeed;
    private bool isDashing = false;
    private bool canMove = true;
    private bool isDead = false;

    public Vector2 Movement => movement;
    public Vector2 LastMovement => lastMovement;
    public bool CanMove => canMove;
    public bool IsDead => isDead;
    public Rigidbody2D RB => rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 

        myAnimator = GetComponentInParent<Animator>();
        if (myAnimator == null) Debug.LogError("Animator NOT FOUND for PlayerControllerCombined!");

        mySpriteRender = GetComponentInParent<SpriteRenderer>();

        playerControls = new PlayerControls();
        playerControls.Movement.Enable();
        playerControls.Combat.Enable();
        playerControls.Dash.Enable();
        playerControls.Inventory.Enable();
    }

    private void Start()
    {
        startingMoveSpeed = moveSpeed;

        playerControls.Dash.Dash.performed += ctx =>
        {
            Sword sword = GetComponentInChildren<Sword>();
            if (sword != null && sword.isAttacking) return;
            Dash();
        };
    }

    private void OnEnable() => playerControls?.Enable();
    private void OnDisable()
    {
        playerControls?.Disable(); // tự động disable tất cả action map
    }

    private void Update()
    {
        if (isDead)
        {
            myAnimator.SetFloat("MoveSpeed", 0f);
            return;
        }

        PlayerInput();
        AdjustAnimatorParams();
    }

    private void FixedUpdate() => Move();

    private void PlayerInput()
    {
        Vector2 pcInput = Vector2.zero;
        if (playerControls != null) pcInput = playerControls.Movement.Move.ReadValue<Vector2>();

        Vector2 joystickInput = moveJoystick != null
            ? new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical)
            : Vector2.zero;

        Vector2 input = pcInput.sqrMagnitude > 0.01f ? pcInput :
                        joystickInput.sqrMagnitude > 0.01f ? joystickInput :
                        Vector2.zero;

        if (input.sqrMagnitude > 0.01f) lastMovement = input;
        movement = input;

        if (myAnimator != null) myAnimator.SetFloat("MoveSpeed", movement.magnitude);

        if (movement.sqrMagnitude > 0.001f && myAnimator != null)
        {
            float angle = Mathf.Atan2(lastMovement.y, lastMovement.x) * Mathf.Rad2Deg - 90f;
            if (angle < 0) angle += 360f;
            myAnimator.SetFloat("DirectionAngle", angle);
        }
    }


    private void Move()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 targetVelocity = movement * moveSpeed;
        rb.velocity = targetVelocity;
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void LockMovement(bool value) => canMove = !value;

    private void AdjustAnimatorParams()
    {
        float speed = movement.magnitude;
        myAnimator.SetFloat("MoveSpeed", speed);

        if (movement.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            if (angle < 0) angle += 360f;
            myAnimator.SetFloat("DirectionAngle", angle);
        }
    }
    public void OnDashButton() { Dash(); }
    private void Dash()
    {
        if (isDead || isDashing) return;

        isDashing = true;
        moveSpeed *= dashSpeed;
        myTrailRenderer.emitting = true;
        StartCoroutine(EndDashRoutine());
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = 0.2f;
        float dashCooldown = 3f;

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
            dashButtonCanvasGroup.alpha = 0.3f;
            dashButtonCanvasGroup.interactable = false;
        }

        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cooldownTime;
            if (dashButtonCanvasGroup) dashButtonCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, t);
            yield return null;
        }

        if (dashButtonCanvasGroup)
        {
            dashButtonCanvasGroup.alpha = 1f;
            dashButtonCanvasGroup.interactable = true;
        }
    }
    public void ResetPlayer()
    {
        isDead = false;
        canMove = true;
        moveSpeed = startingMoveSpeed;
        rb.velocity = Vector2.zero;
        if (myTrailRenderer != null) myTrailRenderer.emitting = false;

        if (myAnimator != null) 
        {
            myAnimator.SetFloat("MoveSpeed", 0f);
            myAnimator.Rebind(); // reset animation
        }
    }

    public void CallSwordDoneAttack()
    {
        Sword sword = GetComponentInChildren<Sword>();
        sword?.DoneAttackingAnimEvent();
    }

    public void PlayDeath()
    {
        if (isDead) return;
        isDead = true;
        canMove = false;
        rb.velocity = Vector2.zero;
        if (myTrailRenderer != null) myTrailRenderer.emitting = false;
        if (myAnimator != null)
        {
            myAnimator.SetFloat("MoveSpeed", 0f);
            myAnimator.SetTrigger("Death");
        }
    }
}
