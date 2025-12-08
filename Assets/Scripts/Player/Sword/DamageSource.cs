using UnityEngine;
using System.Collections.Generic;

public class DamageSource : MonoBehaviour
{
    [SerializeField] int damageAmount = 5;

    private HashSet<int> hitTargets = new HashSet<int>();

    private void OnEnable()
    {
        hitTargets.Clear(); // reset mỗi khi hitbox bật
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyHealth>(out var enemy)) return;

        int id = other.gameObject.GetInstanceID();
        if (hitTargets.Contains(id)) return;

        hitTargets.Add(id);

        enemy.TakeDamage(damageAmount, transform);
    }
}
