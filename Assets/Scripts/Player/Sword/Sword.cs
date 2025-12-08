using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject[] swingHitboxes;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CanvasGroup attackButtonCanvasGroup;

    private PlayerControls playerControls;
    private PlayerControllerCombined playerController;

    private enum SwingDirection { Down, Up }
    private SwingDirection currentSwing = SwingDirection.Down;

    private bool canAttack = true;
    private bool bufferedAttack = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasDealtDamage = false;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerControllerCombined>();

        if (playerAnimator == null && playerController != null)
            playerAnimator = playerController.GetComponent<Animator>();

        if (playerAnimator == null) Debug.LogError("Sword: Không tìm thấy Player Animator!");
        if (playerController == null) Debug.LogError("Sword: Không tìm thấy PlayerControllerCombined!");
    }

    private void EnableSwingHitbox(int swingIndex)
    {
        for (int i = 0; i < swingHitboxes.Length; i++)
            swingHitboxes[i].SetActive(i == swingIndex);
    }

    public void OnAttackButton() => PerformAttack();

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (canAttack) PerformAttack();
        else bufferedAttack = true;
    }

    private void PerformAttack()
    {
    if (!canAttack) return;

    // Lock movement
    playerController.LockMovement(true);
    playerController.RB.constraints = RigidbodyConstraints2D.FreezeAll;

    // Determine swing index
    Vector2 playerDir = playerController.LastMovement;
    int swingIndex = GetSwingIndex(playerDir, currentSwing);
    EnableSwingHitbox(swingIndex);

    int swordLayerIndex = playerAnimator.GetLayerIndex("SwordAttack");
    if (swordLayerIndex != -1 && playerAnimator.GetLayerWeight(swordLayerIndex) == 0f)
        playerAnimator.SetLayerWeight(swordLayerIndex, 1f);

    playerAnimator.SetBool("UsingSword", true);
    playerAnimator.SetFloat("SwingIndex", swingIndex);
    playerAnimator.SetTrigger("AttackTrigger");

    hasDealtDamage = false;   
    isAttacking = true;

    StartCoroutine(AttackCooldownRoutine()); // <-- chỉ 1 lần
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
        if (playerControls != null)
        {
            playerControls.Combat.Attack.performed -= OnAttack;
            playerControls.Disable();
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;

        // Nếu có CanvasGroup cho attack -> mờ đi
        if (attackButtonCanvasGroup)
        {
            attackButtonCanvasGroup.alpha = 0.3f;
            attackButtonCanvasGroup.interactable = false;
        }

        float elapsed = 0f;
        while (elapsed < attackCooldown)
        {
            elapsed += Time.deltaTime;

            if (attackButtonCanvasGroup)
            {
                float t = elapsed / attackCooldown;
                attackButtonCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, t);
            }

            yield return null;
        }

        canAttack = true;

        // Bật lại nút
        if (attackButtonCanvasGroup)
        {
            attackButtonCanvasGroup.alpha = 1f;
            attackButtonCanvasGroup.interactable = true;
        }

        if (bufferedAttack)
        {
            bufferedAttack = false;
            PerformAttack();
        }
    }

    public void DoneAttackingAnimEvent()
    {
        // Tắt tất cả hitbox
        foreach (var h in swingHitboxes)
            h.SetActive(false);

        // Tắt animation layer
        int swordLayerIndex = playerAnimator.GetLayerIndex("SwordAttack");
        if (!bufferedAttack && swordLayerIndex != -1)
            playerAnimator.SetLayerWeight(swordLayerIndex, 0f);

        playerAnimator.SetBool("UsingSword", false);

        isAttacking = false;

        // Unlock movement
        playerController.LockMovement(false);
        playerController.RB.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Nếu có buffered attack -> play ngay sau khi xong
        if (bufferedAttack)
        {
            bufferedAttack = false;
            PerformAttack();
        }

        // Swap combo direction
        currentSwing = (currentSwing == SwingDirection.Down) ? SwingDirection.Up : SwingDirection.Down;
    }

    private int GetSwingIndex(Vector2 playerDir, SwingDirection swing)
    {
        float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int dir =
            angle >= 45 && angle < 135 ? 0 :      // Up
            angle >= 135 && angle < 225 ? 1 :     // Left
            angle >= 225 && angle < 315 ? 2 :     // Down
            3;                                    // Right

        int swingOffset = (swing == SwingDirection.Down) ? 0 : 1;
        return dir * 2 + swingOffset;
    }
}
