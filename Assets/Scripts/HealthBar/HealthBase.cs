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

        Debug.Log($"{gameObject.name} TakeDamage called with dmg={dmg}");

        // --- FLASH ---
        if (flash != null)
            flash.StartFlash();

        // --- KNOCKBACK ---
        if (knockback != null)
            knockback.GetKnockedBack(source ?? transform, knockbackForce);

        // --- DAMAGE POPUP ---
        if (damagePopupPrefab != null && popupCanvas != null)
        {
            // Instantiate toàn bộ GameObject prefab
            GameObject popupGO = Instantiate(damagePopupPrefab.gameObject, popupCanvas);
            popupGO.SetActive(true); // đảm bảo active

            // Lấy component DamageText
            DamageText dt = popupGO.GetComponent<DamageText>();
            dt.Setup(dmg, popupColor);

            Debug.Log("Popup created for damage: " + dmg);
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
            // Nếu đang trong coroutine ẩn, không bật lại
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

        // Hướng canvas về camera
        Vector3 dir = healthBarCanvas.transform.position - Camera.main.transform.position;

        // giữ trục Y để không nghiêng
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            healthBarCanvas.transform.rotation = Quaternion.LookRotation(dir);

            // Đảm bảo text con không bị lật
            foreach (TMP_Text txt in healthBarCanvas.GetComponentsInChildren<TMP_Text>())
            {
                txt.transform.rotation = Quaternion.LookRotation(txt.transform.position - Camera.main.transform.position);
            }
        }
    }




    protected abstract void Die();
}
