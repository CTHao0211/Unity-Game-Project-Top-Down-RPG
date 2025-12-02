using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashSpawnPoint;

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerControllerCombined playerController; // Hoặc PlayerController nếu dùng phiên bản PC/Joystick
    private Transform swordPivot;

    private int lastFacingDirection = 1; // 1 = right, -1 = left

    private enum SwingDirection { Down, Up }
    private SwingDirection currentSwing = SwingDirection.Down;

    private Quaternion defaultRotationRight;
    private Vector3 defaultPositionRight;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerControllerCombined>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        swordPivot = transform.parent;

        if (playerController == null)
            Debug.LogError("Sword: Không tìm thấy PlayerController!");
        if (swordPivot == null)
            Debug.LogError("Sword: Không tìm thấy SwordPivot!");
    }

    private void Start()
    {
        canAttack = true;

        if (swordPivot != null)
        {
            defaultRotationRight = swordPivot.localRotation;
            defaultPositionRight = swordPivot.localPosition;
        }
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
    private bool canAttack = true;
    public void EnableAttack()
    {
        canAttack = true;
    }

    [SerializeField] private float attackCooldown = 0.5f; // thời gian animation

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!canAttack) return;

        // Trigger animation
        if (myAnimator != null)
            myAnimator.SetTrigger("Attack");

        // Spawn Slash chỉ khi Swing Down
        if (currentSwing == SwingDirection.Down)
            SpawnSlashEffect();

        // Toggle Swing cho animation lần tiếp theo
        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;

        // Lock attack trong cooldown
        StartCoroutine(AttackCooldownRoutine());
    }



    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown); // attackCooldown ≈ thời gian animation
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




    private void LateUpdate()
    {
        FaceMoveDirection();
    }

    private void FaceMoveDirection()
    {
        if (playerController == null || swordPivot == null) return;

        Vector2 dir = playerController.Movement;

        // Chỉ update khi di chuyển theo X
        if (Mathf.Abs(dir.x) > 0.01f)
            lastFacingDirection = (dir.x < 0f) ? -1 : 1;

        // Flip swordPivot dựa theo hướng
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
            swordPivot.localScale = new Vector3(1, 1, 1);
            swordPivot.localRotation = defaultRotationRight;
            swordPivot.localPosition = defaultPositionRight;
        }
    }
}
