using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashSpawnPoint;
    [SerializeField] private PolygonCollider2D weaponCollider;   // kéo từ "Weapon Collider"
    [SerializeField] private float hitboxActiveTime = 0.15f;     // thời gian collider bật khi chém
    [SerializeField] private float attackCooldown = 0.5f;        // thời gian giữa 2 lần chém

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerControllerCombined playerController;
    private Transform swordPivot;
    private GameObject currentSlash;   // thêm dòng này


    private int lastFacingDirection = 1; // 1 = right, -1 = left

    private enum SwingDirection { Down, Up }
    private SwingDirection currentSwing = SwingDirection.Down;

    private bool canAttack = true;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerControllerCombined>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        swordPivot = transform.parent;    // chính là WeaponPivot

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

        // ẨN HOÀN TOÀN HITBOX LÚC CHƯA ĐÁNH
        if (weaponCollider != null)
            weaponCollider.gameObject.SetActive(false);
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

        if (myAnimator != null)
            myAnimator.SetTrigger("Attack");
        if (weaponCollider !=null)
            weaponCollider.gameObject.SetActive(true);
        if (currentSwing == SwingDirection.Down)
            SpawnSlashEffect();

        if (weaponCollider != null)
            StartCoroutine(HitboxRoutine());

        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;

        StartCoroutine(AttackCooldownRoutine());
    }

    public void DoneAttackingAnimEvent()
    {
        if (weaponCollider != null)
            weaponCollider.gameObject.SetActive(false);

        // (tuỳ bạn) có thể destroy slash sau khi đánh xong
        if (currentSlash != null)
        {
            Destroy(currentSlash);
            currentSlash = null;
        }
    }

    public void SwingUpFlipAnimEvent()
    {
        if (currentSlash == null) return;

        // Up: lật dọc (Y âm), trái/phải theo lastFacingDirection
        currentSlash.transform.localScale = new Vector3(
            lastFacingDirection,   // 1 = right, -1 = left
            -1f,                   // up: lật dọc
            1f
        );
    }

    public void SwingDownFlipAnimEvent()
    {
        if (currentSlash == null) return;

        // Down: Y dương, trái/phải theo lastFacingDirection
        currentSlash.transform.localScale = new Vector3(
            lastFacingDirection,
            1f,                    // down: bình thường
            1f
        );
    }


    private IEnumerator HitboxRoutine()
    {
        // CHỈ LÚC NÀY WEAPON COLLIDER MỚI XUẤT HIỆN
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

        // Tạo slash và lưu lại instance
        currentSlash = Instantiate(slashEffectPrefab, slashSpawnPoint.position, Quaternion.identity);

        // Cho slash bám theo tay
        currentSlash.transform.SetParent(slashSpawnPoint, worldPositionStays: true);

        // Không set scale ở đây, để animation event lo flip
        currentSlash.transform.localScale = Vector3.one;
        currentSlash.transform.rotation = Quaternion.identity;
    }


    private void LateUpdate()
    {
        FaceMoveDirection();
    }

    private void FaceMoveDirection()
    {
        if (playerController == null || swordPivot == null) return;

        Vector2 dir = playerController.Movement;

        if (Mathf.Abs(dir.x) > 0.01f)
            lastFacingDirection = (dir.x < 0f) ? -1 : 1;

        // Flip toàn bộ WeaponPivot → Sword, SlashSpawnPoint, Weapon Collider đều quay theo
        Vector3 scale = swordPivot.localScale;
        scale.x = lastFacingDirection;
        swordPivot.localScale = scale;
    }
}
