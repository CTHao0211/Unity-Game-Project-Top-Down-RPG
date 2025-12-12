using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI (Canvas Scene)")]
    public Image fillBar;       // Gán FillBar Image từ Canvas
    public TMP_Text healthText; // Gán TMP_Text từ Canvas

    [Header("Effects")]
    public Flash flash;         // Flash component (đổi màu)
    public Knockback knockback; // Knockback component

    [Header("Animation")]
    public Animator animator;   // Animator nếu muốn trigger "Hit"

    [Header("Damage Popup")]
    public DamageText damagePopupPrefab; // Gán prefab DamageText
    public Transform popupCanvas;        // Canvas dùng hiển thị popup

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    [Header("Invulnerability")]
    public bool useInvulnerability = true;
    public float invulnTime = 0.3f;
    private bool isInvulnerable = false;

    private bool isDead = false;
    private void Awake()
    {
        flash = flash ?? GetComponent<Flash>();
        knockback = knockback ?? GetComponent<Knockback>();

        // TÌM Animator Ở TRÊN CHA (PlayerControllerCombined hoặc Player)
        if (animator == null)
            animator = GetComponentInParent<Animator>();

        // Nếu chưa gán popupCanvas, tìm Canvas trong scene
        if (popupCanvas == null)
        {
            Canvas c = FindObjectOfType<Canvas>();
            if (c != null)
                popupCanvas = c.transform;
        }
    }


    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// Nhận damage
    /// </summary>
    public void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        if (isDead) return;
        if (isInvulnerable) return;

        // set invulnerable window nếu cần
        if (useInvulnerability)
        {
            isInvulnerable = true;
            StartCoroutine(InvulnRoutine());
        }

        currentHealth -= dmg;
        if (currentHealth < 0)
            currentHealth = 0;

        // Flash khi bị damage
        flash.FlashWhite();


        //// Animator trigger (nếu muốn)
        //animator.SetTrigger("Hit");


        // Knockback nếu có nguồn damage
        if (source != null)
            knockback?.GetKnockedBack(source, knockbackForce);

        // Damage Popup
        if (damagePopupPrefab != null && popupCanvas != null)
        {
            DamageText dt = Instantiate(damagePopupPrefab, popupCanvas);
            dt.transform.localScale = Vector3.one;
            dt.transform.localPosition = Vector3.zero;
            dt.Setup(dmg, popupColor);
        }

        UpdateHealthUI();

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    private IEnumerator InvulnRoutine()
    {
        yield return new WaitForSeconds(invulnTime);
        isInvulnerable = false;
    }

    /// <summary>
    /// Hồi máu
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    /// <summary>
    /// Cập nhật thanh máu
    /// </summary>
    public void UpdateHealthUI()
    {
        if (fillBar != null)
            fillBar.fillAmount = (float)currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    /// <summary>
    /// Khi Player chết
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died!");

        if (PlayerControllerCombined.instance != null)
            PlayerControllerCombined.instance.PlayDeath();
    }

    public void OnDeathAnimationEnd()
    {
        if (GameManager.instance != null)
            GameManager.instance.GameOver();
    }
}
