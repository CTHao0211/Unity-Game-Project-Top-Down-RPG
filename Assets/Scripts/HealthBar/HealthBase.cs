using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class HealthBase : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private bool faceCamera = true;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Health Bar UI")]
    [SerializeField] protected Canvas healthBarCanvas;
    [SerializeField] protected Image fillBar;
    [SerializeField] protected TMP_Text healthText;

    [Header("Effects")]
    [SerializeField] protected Flash flash;
    [SerializeField] protected Knockback knockback;
    [SerializeField] protected float knockbackForce = 12f;

    [Header("Damage Popup")]
    [SerializeField] protected DamageText damagePopupPrefab;
    [SerializeField] protected Transform popupCanvas;
    public bool loadedFromSave = false;
    protected virtual void Awake()
    {
        if (!loadedFromSave)
            currentHealth = maxHealth;

        if (healthBarCanvas == null)
            healthBarCanvas = GetComponentInChildren<Canvas>(true);

        if (popupCanvas == null)
            popupCanvas = healthBarCanvas.transform;

        // Find HealthBar
        Transform barRoot = healthBarCanvas.transform.Find("HealthBar");
        if (barRoot == null)
            Debug.LogError($"{name}: Không tìm thấy object 'HealthBar' trong Canvas!");

        if (fillBar == null)
            fillBar = barRoot.Find("FillBar")?.GetComponent<Image>();

        if (fillBar == null)
            Debug.LogError($"{name}: Không tìm thấy FillBar!");

        if (healthText == null)
            healthText = barRoot.GetComponentInChildren<TMP_Text>(true);

        flash = flash ?? GetComponent<Flash>();
        knockback = knockback ?? GetComponent<Knockback>();

        UpdateHealthBar();
    }
    public void ApplyLoadedHP(int hp)
    {
        loadedFromSave = true;
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);
        UpdateHealthBar();
    }

    /// <summary>
    /// Gây damage
    /// </summary>
    public virtual void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;
        AnimalAudio audio = GetComponent<AnimalAudio>();
        if(audio != null)
        audio.PlayHitSound();
        flash.FlashWhite();
        if (source != null)
            knockback?.GetKnockedBack(source, knockbackForce);

        // === SPAWN DAMAGE POPUP ===
        if (damagePopupPrefab != null && popupCanvas != null)
        {
            DamageText dt = Instantiate(damagePopupPrefab, popupCanvas);
            dt.transform.localScale = Vector3.one;
            dt.Setup(dmg, popupColor);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }


    protected virtual void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;

        if (fillBar != null)
            fillBar.fillAmount = ratio;

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            // Đảm bảo healthText hiển thị trên FillBar
            healthText.transform.SetAsLastSibling();
        }

        if (healthBarCanvas != null && hideWhenFull)
            healthBarCanvas.enabled = ratio < 1f;
    }

    protected virtual void LateUpdate()
    {
        if (!faceCamera || healthBarCanvas == null) return;

        healthBarCanvas.transform.LookAt(Camera.main.transform);
        healthBarCanvas.transform.Rotate(0, 180, 0);
    }

    protected abstract void Die();
}
