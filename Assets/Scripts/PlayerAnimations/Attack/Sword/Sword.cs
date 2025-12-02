using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashSpawnPoint; 
    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private Transform swordPivot;
    private Vector2 lastSwingDirection = Vector2.zero;
    private int lastFacingDirection = 1;
    private enum SwingDirection
    {
        Down,
        Up
    }

    private SwingDirection currentSwing = SwingDirection.Down; // bắt đầu là SwingDown

    private Quaternion defaultRotationRight;
    private Vector3 defaultPositionRight; // Thêm vị trí mặc định

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
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
        if (swordPivot != null)
        {
            defaultRotationRight = swordPivot.localRotation;
            defaultPositionRight = swordPivot.localPosition; // Lưu vị trí gốc
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Combat.Attack.performed -= OnAttack; // tránh duplicate
        playerControls.Combat.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        playerControls.Combat.Attack.performed -= OnAttack;
        playerControls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("Attack");

        // Spawn Slash **dựa vào Swing hiện tại**
        SpawnSlashEffect();

        // Sau đó toggle Swing cho lần nhấn tiếp theo
        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;
    }



    private void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null || slashSpawnPoint == null) return;

        // Chỉ spawn khi SwingDown
        if (currentSwing != SwingDirection.Down) return;

        GameObject slash = Instantiate(slashEffectPrefab, slashSpawnPoint.position, Quaternion.identity);

        // Flip theo hướng nhìn cuối (trái/phải)
        slash.transform.localScale = (lastFacingDirection < 0) ? new Vector3(-1, 1, 1) : Vector3.one;
        slash.transform.rotation = Quaternion.identity;
    }






    private void Update()
    {
        FaceMoveDirection();
    }

    private void FaceMoveDirection()
    {
        if (playerController == null || swordPivot == null) return;
        
        Vector2 dir = playerController.Movement;
        
    if (dir.sqrMagnitude > 0.01f)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            lastFacingDirection = (dir.x < 0f) ? -1 : 1;
        }
        // Nếu chỉ muốn Slash xoay theo Y khi đứng yên, không cần thay đổi lastFacingDirection
    }

        
        if (lastFacingDirection < 0)
        {
            // Trái: flip scale + đảo X position
            swordPivot.localScale = new Vector3(-1, 1, 1);
            swordPivot.localRotation = defaultRotationRight;
            swordPivot.localPosition = new Vector3(
                -defaultPositionRight.x, // Đảo dấu X
                defaultPositionRight.y,
                defaultPositionRight.z
            );
        }
        else
        {
            // Phải: giữ nguyên
            swordPivot.localScale = new Vector3(1, 1, 1);
            swordPivot.localRotation = defaultRotationRight;
            swordPivot.localPosition = defaultPositionRight;
        }
    }
}
