using System.Collections;
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

    [Header("Death VFX")]
    [SerializeField] protected GameObject deathVFXPrefab;

    [SerializeField] private float hideDelay = 5f; // 5 giây sau khi nhận damage
    private Coroutine hideRoutine;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        if (flash == null) flash = GetComponent<Flash>();
        if (knockback == null) knockback = GetComponent<Knockback>();

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

        UpdateHealthBar();
    }

    /// <summary>
    /// Gây damage
    /// </summary>
    public virtual void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        // --- FLASH ---
        if (flash != null)
            flash.StartFlash();

        // --- KNOCKBACK ---
        if (knockback != null)
            knockback.GetKnockedBack(source ?? transform, knockbackForce);

        // --- DAMAGE POPUP ---
        if (damagePopupPrefab != null && popupCanvas != null)
        {
            GameObject popupGO = Instantiate(damagePopupPrefab.gameObject, popupCanvas);
            popupGO.SetActive(true);

            DamageText dt = popupGO.GetComponent<DamageText>();
            dt.Setup(dmg, popupColor);
        }

        // --- UPDATE HEALTHBAR ---
        UpdateHealthBar();

        // --- ẨN HEALTHBAR SAU hideDelay ---
        if (currentHealth > 0 && hideWhenFull)
        {
            if (hideRoutine != null)
                StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideHealthBarAfterDelay());
        }

        // --- KIỂM TRA CHẾT ---
        if (currentHealth <= 0)
            DieWithVFX();
    }

    private void DieWithVFX()
    {
        // --- SPAWN DEATH VFX ---
        if (deathVFXPrefab != null)
        {
            GameObject vfx = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Debug.Log($"[HealthBase] Spawn VFX: {vfx.name} tại {vfx.transform.position}");
        }

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
            healthText.transform.SetAsLastSibling();
        }

        if (healthBarCanvas != null && hideWhenFull)
        {
            if (!isHiding)
            {
                healthBarCanvas.enabled = ratio < 1f;
            }
        }
    }

    private bool isHiding = false;
    private IEnumerator HideHealthBarAfterDelay()
    {
        isHiding = true;
        yield return new WaitForSeconds(hideDelay);
        if (healthBarCanvas != null)
            healthBarCanvas.enabled = false;
        isHiding = false;
    }

    protected virtual void LateUpdate()
    {
        if (!faceCamera || healthBarCanvas == null) return;

        Vector3 dir = healthBarCanvas.transform.position - Camera.main.transform.position;
        dir.y = 0f; // giữ healthbar thẳng
        if (dir.sqrMagnitude > 0.001f)
        {
            healthBarCanvas.transform.rotation = Quaternion.LookRotation(dir);

            // Quay text con riêng để không bị lộn ngược
            foreach (TMP_Text txt in healthBarCanvas.GetComponentsInChildren<TMP_Text>())
            {
                txt.transform.rotation = Quaternion.LookRotation(txt.transform.position - Camera.main.transform.position);
            }
        }
    }

    // --- Hàm abstract để lớp con override ---
    protected abstract void Die();
}
