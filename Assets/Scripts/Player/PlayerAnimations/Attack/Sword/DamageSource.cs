using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    // Sát thương cơ bản
    [SerializeField] private int damageAmount = 5;

    // Loại đòn đánh được gán từ Sword
    public enum AttackType { Normal, SlashCrit }
    public AttackType attackType = AttackType.Normal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            int finalDamage = damageAmount;


            // Nếu là slash (down swing), gây sát thương ngẫu nhiên 1–3
            if (attackType == AttackType.SlashCrit)
            {
                finalDamage = Random.Range(1, 4); // 1–3 damage
            }

            enemyHealth.TakeDamage(finalDamage, transform);
        }
    }
}

