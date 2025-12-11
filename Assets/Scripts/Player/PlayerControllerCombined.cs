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
    [SerializeField] private Rigidbody2D rb; 

    public Rigidbody2D RB => rb;

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
    playerControls.Dash.Dash.performed += ctx => {
        Sword sword = GetComponentInChildren<Sword>();
        if (sword != null && sword.isAttacking) return; // Không dash khi đang attack
        Dash();
    };
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
        if (isDead)          // ⬅️ Chết rồi thì khỏi xử lý input
        {
            myAnimator.SetFloat("MoveSpeed", 0f);
            return;
        }

        PlayerInput();
        AdjustAnimatorParams();
    }



    private void FixedUpdate()
    {
        Move();
    }

    private Vector2 lastMovement = Vector2.down;

    public Vector2 LastMovement => lastMovement;

    private void PlayerInput()
    {
        Vector2 pcInput = playerControls.Movement.Move.ReadValue<Vector2>();
        Vector2 joystickInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        Vector2 input = pcInput.sqrMagnitude > 0.01f ? pcInput :
                        joystickInput.sqrMagnitude > 0.01f ? joystickInput :
                        Vector2.zero;

        if(input.sqrMagnitude > 0.01f)
            lastMovement = input;

        movement = input;

        myAnimator.SetFloat("MoveSpeed", movement.magnitude);
        
        float angle = Mathf.Atan2(lastMovement.y, lastMovement.x) * Mathf.Rad2Deg - 90f;
        if(angle < 0) angle += 360f;
        myAnimator.SetFloat("DirectionAngle", angle);
    }
    private bool canMove = true; // Mặc định được di chuyển
    public bool CanMove => canMove;

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Move()
    {
        if(!canMove){
        rb.velocity = Vector2.zero;
        return; }// Nếu đang attack, không di chuyển
        Vector2 targetVelocity = movement * moveSpeed;
        rb.velocity = targetVelocity;
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void LockMovement(bool value)
    {
        canMove = !value;
    }



    // Thay enum và integer bằng float
    private void AdjustPlayerFacingDirection()
    {
        if (movement.sqrMagnitude < 0.001f)
            return;

        // Tính góc từ vector movement
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        angle -= 90f; // nếu muốn 0° = Down
        if (angle < 0f) angle += 360f;

        // Gán cho Animator float
        myAnimator.SetFloat("DirectionAngle", angle);
    }
    private void AdjustAnimatorParams()
    {
        float speed = movement.magnitude; // 0 = Idle, >0 = Run
        myAnimator.SetFloat("MoveSpeed", speed);

        if (movement.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            angle -= 90f; // 0° = Down
            if (angle < 0) angle += 360f;
            myAnimator.SetFloat("DirectionAngle", angle);
        }
    }


    private void Dash()
    {
        if (isDead) return;  // ⬅️ Chết thì không dash

        if (!isDashing)
        {
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
    public void CallSwordDoneAttack()
    {
        Sword sword = GetComponentInChildren<Sword>();
        if (sword != null)
            sword.DoneAttackingAnimEvent();
    }

    public void PlayDeath()
    {
        if (isDead) return;   // tránh gọi nhiều lần

        isDead = true;
        canMove = false;

        // Dừng hẳn player
        if (rb != null)
            rb.velocity = Vector2.zero;

        // Tắt trail nếu đang dash
        if (myTrailRenderer != null)
            myTrailRenderer.emitting = false;

        // Gọi animation Death
        if (myAnimator != null)
        {
            myAnimator.SetFloat("MoveSpeed", 0f);
            myAnimator.SetTrigger("Death");   // ⬅️ TRÙNG TÊN PARAM Trong Animator nhé
        }

        // (Optional) tắt collider để không bị đánh nữa
        // Collider2D col = GetComponent<Collider2D>();
        // if (col != null) col.enabled = false;
    }


}
