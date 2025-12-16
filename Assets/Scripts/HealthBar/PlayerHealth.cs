// PlayerHealth.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Image fillBar;
    public TMP_Text healthText;

    [Header("Effects")]
    public Flash flash;
    public Knockback knockback;

    [Header("Animation")]
    public Animator animator;

    [Header("Damage Popup")]
    public DamageText damagePopupPrefab;
    public Transform popupCanvas;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    [Header("Invulnerability")]
    public bool useInvulnerability = true;
    public float invulnTime = 0.3f;
    private bool isInvulnerable = false;
    private bool loadedFromSave = false;
    private bool isDead = false;

    private void Awake()
    {
        flash = flash ?? GetComponent<Flash>();
        knockback = knockback ?? GetComponent<Knockback>();
        if (animator == null) animator = GetComponentInParent<Animator>();

        if (popupCanvas == null)
        {
            Canvas c = FindObjectOfType<Canvas>();
            if (c != null) popupCanvas = c.transform;
        }
    }

    private void Start()
    {
        if (!loadedFromSave) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int dmg, Transform source = null, Color? popupColor = null, bool canKnockback = true, bool ignoreInvuln = false)
    {
        if (isDead) return;
        if (isInvulnerable && !ignoreInvuln) return;

        if (useInvulnerability)
        {
            isInvulnerable = true;
            StartCoroutine(InvulnRoutine());
        }

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        flash?.FlashWhite();
        if (canKnockback && source != null) knockback?.GetKnockedBack(source, knockbackForce);

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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (fillBar != null)
            fillBar.fillAmount = (float)currentHealth / maxHealth;
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    private void Die()
    {
        Debug.Log("Player died!");

        if (PlayerControllerCombined.Instance != null)
            PlayerControllerCombined.Instance.PlayDeath();

        var gm = GameManager.instance ?? FindObjectOfType<GameManager>();
        gm?.GameOver();
    }

    public void ApplyLoadedHP(int hp)
    {
        loadedFromSave = true;
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);
        UpdateHealthUI();
    }

    public void OnDeathAnimationEnd()
    {
        var gm = GameManager.instance ?? FindObjectOfType<GameManager>();
        gm?.GameOver();
    }

    public void ApplyLoadedHPDelayed(int hp)
    {
        StartCoroutine(ApplyNextFrame(hp));
    }

    private IEnumerator ApplyNextFrame(int hp)
    {
        yield return null;
        ApplyLoadedHP(hp);
    }
}
