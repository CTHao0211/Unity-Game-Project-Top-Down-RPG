using UnityEngine;

public class SwordMobile : MonoBehaviour
{
    private Animator myAnimator;
    private PlayerControllerMobile playerController;   // script move mobile
    private SpriteRenderer swordSprite;

    // vị trí mặc định khi nhân vật nhìn SANG PHẢI
    private Vector3 defaultLocalPos;

    private void Awake()
    {
        // Lấy PlayerControllerMobile từ cha
        playerController = GetComponentInParent<PlayerControllerMobile>();
        myAnimator = GetComponent<Animator>();
        swordSprite = GetComponent<SpriteRenderer>();

        if (playerController == null)
            Debug.LogError("SwordMobile: Không tìm thấy PlayerControllerMobile trên cha!");

        if (swordSprite == null)
            Debug.LogError("SwordMobile: Không tìm thấy SpriteRenderer trên kiếm!");
    }

    private void Start()
    {
        // Giả định lúc start nhân vật đang nhìn sang PHẢI
        defaultLocalPos = transform.localPosition;
    }

    private void Update()
    {
        FaceMoveDirection();
    }

    // === HÀM GỌI TỪ NÚT ATTACK ===
    public void OnAttackButton()
    {
        Attack();
    }

    private void Attack()
    {
        if (myAnimator != null)
        {
            // Giống PC: trigger "Attack"
            myAnimator.SetTrigger("Attack");
        }
    }

    private void FaceMoveDirection()
    {
        if (playerController == null || swordSprite == null)
            return;

        Vector2 dir = playerController.Movement;   // Movement lấy từ script mobile

        // Đứng yên thì giữ nguyên
        if (dir.sqrMagnitude < 0.01f)
            return;

        if (dir.x < 0f)
        {
            // NHÂN VẬT QUAY TRÁI:
            // - Kiếm nhảy sang bên trái (x âm)
            // - Kiếm flipX = true
            transform.localPosition = new Vector3(
                -Mathf.Abs(defaultLocalPos.x),
                defaultLocalPos.y,
                defaultLocalPos.z
            );
            swordSprite.flipX = true;
        }
        else if (dir.x > 0f)
        {
            // NHÂN VẬT QUAY PHẢI:
            // - Kiếm về vị trí mặc định bên phải (x dương)
            // - Bỏ flip
            transform.localPosition = new Vector3(
                Mathf.Abs(defaultLocalPos.x),
                defaultLocalPos.y,
                defaultLocalPos.z
            );
            swordSprite.flipX = false;
        }
    }
}
