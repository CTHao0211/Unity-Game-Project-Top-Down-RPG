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
    public Flash flash;         // Gán Flash component (SpriteRenderer)
    public Knockback knockback; // Gán Knockback component nếu muốn

    [Header("Damage Popup")]
    public DamageText damagePopupPrefab; // Gán prefab DamageText
    public Transform popupCanvas;        // Canvas Scene dùng hiển thị popup

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    private void Awake()
    {
        // Nếu chưa gán flash/knockback, lấy từ component
        flash = flash ?? GetComponent<Flash>();
        knockback = knockback ?? GetComponent<Knockback>();

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
        currentHealth = maxHealth; // Thêm dòng này
        UpdateHealthUI();
    }


    /// <summary>
    /// Nhận damage
    /// </summary>
    public void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        currentHealth -= dmg;
        if (currentHealth < 0)
            currentHealth = 0;

        // Flash khi bị damage
        flash?.StartFlash();

        // Knockback nếu có nguồn damage
        if (source != null)
            knockback?.GetKnockedBack(source, knockbackForce);

        // Damage Popup
        if (damagePopupPrefab != null && popupCanvas != null)
        {
            DamageText dt = Instantiate(damagePopupPrefab, popupCanvas);
            dt.transform.localScale = Vector3.one;
            dt.transform.localPosition = Vector3.zero; // sẽ đặt lại position nếu muốn
            dt.Setup(dmg, popupColor);
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
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
    private void UpdateHealthUI()
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
        // Disable Player Controller
        PlayerControllerCombined.instance.enabled = false;

        // TODO: Hiển thị Game Over UI
    }
}
