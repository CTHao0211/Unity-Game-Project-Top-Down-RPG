using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 4;

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
            Debug.Log("Enemy chết!");
            Destroy(gameObject);
        }
    }
}
