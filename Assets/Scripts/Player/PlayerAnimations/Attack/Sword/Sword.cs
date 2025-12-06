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

    private int lastFacingDirection = 1; // 1 = right, -1 = left

    private enum SwingDirection { Down, Up }
    private SwingDirection currentSwing = SwingDirection.Down;

    private bool canAttack = true;

    private Quaternion defaultRotationRight;
    private Vector3 defaultPositionRight;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerControllerCombined>();
        myAnimator = GetComponent<Animator>();
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

        // // Ẩn hitbox
        // if (weaponCollider != null)
        //     weaponCollider.gameObject.SetActive(false);
        weaponCollider.enabled = false;

        // ⭐ LƯU VỊ TRÍ / ROTATION GỐC BÊN PHẢI
        defaultRotationRight = swordPivot.localRotation;
        defaultPositionRight = swordPivot.localPosition;
    }
        // === HÀM GỌI TỪ NÚT ATTACK ===
        public void OnAttackButton()
        {
            Attack();
        }

    private void Attack(){
        PerformAttack();
    }
    private void PerformAttack()
    {
        if (!canAttack) return;

        myAnimator?.SetTrigger("Attack");

        // Lấy DamageSource từ collider
        DamageSource dmg = weaponCollider.GetComponent<DamageSource>();
        if (dmg != null)
        {
            if (currentSwing == SwingDirection.Down)
                dmg.attackType = DamageSource.AttackType.SlashCrit;
            else
                dmg.attackType = DamageSource.AttackType.Normal;
        }

        if (currentSwing == SwingDirection.Down)
            SpawnSlashEffect();

        StartCoroutine(HitboxRoutine());

        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;

        StartCoroutine(AttackCooldownRoutine());
    }

    private void OnEnable()
    {
        if (playerControls == null)
            playerControls = new PlayerControls();

        playerControls.Enable();
        playerControls.Combat.Attack.performed += OnAttack;
    }


    private void OnDisable()
    {
        playerControls.Combat.Attack.performed -= OnAttack;
        playerControls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx){
    PerformAttack();
    }

    private IEnumerator HitboxRoutine()
    {
        float timer = 0f;

        weaponCollider.enabled = true; // bật collider (ko tắt GameObject nữa!)

        while (timer < hitboxActiveTime)
        {
            // collider đi theo slash
            weaponCollider.transform.position = slashSpawnPoint.position;
            weaponCollider.transform.rotation = slashSpawnPoint.rotation;

            timer += Time.deltaTime;
            yield return null;
        }

        weaponCollider.enabled = false;
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
            weaponCollider.enabled = false;
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
