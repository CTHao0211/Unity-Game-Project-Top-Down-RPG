using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashSpawnPoint;
    [SerializeField] private PolygonCollider2D weaponCollider;
    [SerializeField] private float hitboxActiveTime = 0.15f;
    [SerializeField] private float attackCooldown = 0.5f;

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerControllerCombined playerController;
    private Transform swordPivot;
    private GameObject currentSlash;

    private int lastFacingDirection = 1; // 1 = right, -1 = left

    private enum SwingDirection { Down, Up }
    private SwingDirection currentSwing = SwingDirection.Down;

    private bool canAttack = true;

    // ⭐ THÊM HAI BIẾN NÀY (giống code tham khảo)
    private Quaternion defaultRotationRight;
    private Vector3 defaultPositionRight;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerControllerCombined>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        swordPivot = transform.parent;

        if (playerController == null)
            Debug.LogError("Sword: Không tìm thấy PlayerControllerCombined!");
        if (swordPivot == null)
            Debug.LogError("Sword: Không tìm thấy WeaponPivot!");
        if (weaponCollider == null)
            Debug.LogError("Sword: Chưa gán WeaponCollider!");
    }

    private void Start()
    {
        canAttack = true;

        // Ẩn hitbox
        if (weaponCollider != null)
            weaponCollider.gameObject.SetActive(false);

        // ⭐ LƯU VỊ TRÍ / ROTATION GỐC BÊN PHẢI
        defaultRotationRight = swordPivot.localRotation;
        defaultPositionRight = swordPivot.localPosition;
    }
        // === HÀM GỌI TỪ NÚT ATTACK ===
        public void OnAttackButton()
        {
            Attack();
        }

    private void Attack()
    {
        if (!canAttack) return;

        myAnimator?.SetTrigger("Attack");

        // Spawn Slash khi swing xuống
        if (currentSwing == SwingDirection.Down)
            SpawnSlashEffect();

        // Hitbox bật/tắt
        StartCoroutine(HitboxRoutine());

        // Đổi swing cho lần kế
        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;

        // Cooldown
        StartCoroutine(AttackCooldownRoutine());
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Combat.Attack.performed -= OnAttack;
        playerControls.Combat.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        playerControls.Combat.Attack.performed -= OnAttack;
        playerControls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;

        myAnimator?.SetTrigger("Attack");

        // ⭐ Slash chỉ xuất hiện khi Swing Down
        if (currentSwing == SwingDirection.Down)
            SpawnSlashEffect();

        // Hitbox bật theo thời gian
        StartCoroutine(HitboxRoutine());

        // Đổi swing cho lần tới
        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;

        // cooldown
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator HitboxRoutine()
    {
        weaponCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitboxActiveTime);
        weaponCollider.gameObject.SetActive(false);
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null || slashSpawnPoint == null) return;

        // Spawn tại vị trí sword hiện tại
        GameObject slash = Instantiate(slashEffectPrefab, slashSpawnPoint.position, Quaternion.identity);

        // Đặt scale flip theo hướng player
        slash.transform.localScale = (lastFacingDirection < 0) ? new Vector3(-1, 1, 1) : Vector3.one;

        // Nếu Slash prefab có Rigidbody2D hoặc Animator, hãy đảm bảo nó không parent theo player
        slash.transform.parent = slashSpawnPoint;


        // Rotation reset
        slash.transform.rotation = Quaternion.identity;
    }

    public void DoneAttackingAnimEvent()
    {
        if (weaponCollider != null)
            weaponCollider.gameObject.SetActive(false);

        if (currentSlash != null)
        {
            Destroy(currentSlash);
            currentSlash = null;
        }
    }

    private void LateUpdate()
    {
        FaceMoveDirection();
    }

    private void FaceMoveDirection()
    {
        if (playerController == null || swordPivot == null) return;

        Vector2 dir = playerController.Movement;

        // Update hướng
        if (Mathf.Abs(dir.x) > 0.01f)
            lastFacingDirection = dir.x < 0 ? -1 : 1;

        // ⭐ Flip giống code tham chiếu
        if (lastFacingDirection < 0)
        {
            swordPivot.localScale = new Vector3(-1, 1, 1);
            swordPivot.localRotation = defaultRotationRight;
            swordPivot.localPosition = new Vector3(
                -defaultPositionRight.x,
                defaultPositionRight.y,
                defaultPositionRight.z
            );
        }
        else
        {
            swordPivot.localScale = Vector3.one;
            swordPivot.localRotation = defaultRotationRight;
            swordPivot.localPosition = defaultPositionRight;
        }
    }
}
