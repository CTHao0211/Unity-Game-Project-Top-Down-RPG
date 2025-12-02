using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private Transform swordPivot;
    
    private int lastFacingDirection = 1;
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
            if (dir.x < 0f)
            {
                lastFacingDirection = -1;
            }
            else if (dir.x > 0f)
            {
                lastFacingDirection = 1;
            }
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
