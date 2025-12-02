using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private SpriteRenderer swordSprite;

    // vị trí mặc định của kiếm khi đứng nhìn sang PHẢI
    private Vector3 defaultLocalPos;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        myAnimator = GetComponent<Animator>();
        swordSprite = GetComponent<SpriteRenderer>();
        playerControls = new PlayerControls();

        if (playerController == null)
            Debug.LogError("Sword: Không tìm thấy PlayerController trên cha!");
        if (swordSprite == null)
            Debug.LogError("Sword: Không tìm thấy SpriteRenderer trên Sword!");
    }

    private void Start()
    {
        // Lưu lại localPosition hiện tại (đã chỉnh đúng tay phải)
        defaultLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Combat.Attack.started += OnAttack;
    }

    private void OnDisable()
    {
        playerControls.Combat.Attack.started -= OnAttack;
        playerControls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        Attack();
    }

    private void Attack()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("Attack");
    }

    private void Update()
    {
        FaceMoveDirection();
    }

    // Chỉ đổi trái/phải, KHÔNG xoay, KHÔNG đổi độ cao
    private void FaceMoveDirection()
    {
        if (playerController == null || swordSprite == null) return;

        Vector2 dir = playerController.Movement;

        // đứng yên thì giữ nguyên
        if (dir.sqrMagnitude < 0.01f)
            return;

        if (dir.x < 0f)
        {
            // sang TRÁI: đảo localPosition.x + flip sprite
            transform.localPosition = new Vector3(
                -Mathf.Abs(defaultLocalPos.x),
                defaultLocalPos.y,
                defaultLocalPos.z
            );
            swordSprite.flipX = true;
        }
        else if (dir.x > 0f)
        {
            // sang PHẢI: trả về vị trí mặc định + bỏ flip
            transform.localPosition = new Vector3(
                Mathf.Abs(defaultLocalPos.x),
                defaultLocalPos.y,
                defaultLocalPos.z
            );
            swordSprite.flipX = false;
        }
    }
}
