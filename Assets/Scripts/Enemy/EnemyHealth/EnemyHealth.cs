using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 4;
    [SerializeField] protected GameObject deathVFXPrefab;

    private int currentHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[EnemyHealth] Bị trúng đòn, máu còn: {currentHealth}");

        // Flash
        if (flash != null)
        {
            Debug.Log("[EnemyHealth] Gọi StartFlash");
            flash.StartFlash();
        }

        // Knockback
        if (knockback != null && PlayerControllerCombined.instance != null)
        {
            knockback.GetKnockedBack(PlayerControllerCombined.instance.transform, 15f);
        }

        // Kiểm tra chết
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("[EnemyHealth] Enemy chết! Spawn VFX...");

            if (deathVFXPrefab != null)
            {
                GameObject vfx =
                    Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
                Debug.Log($"[EnemyHealth] Đã spawn VFX: {vfx.name} tại {vfx.transform.position}");
            }
            else
            {
                Debug.LogWarning("[EnemyHealth] deathVFXPrefab đang NULL!");
            }

            Destroy(gameObject);
        }
    }


}
