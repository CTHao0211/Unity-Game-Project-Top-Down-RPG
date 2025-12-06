using UnityEngine;

public class EnemyHealth : HealthBase
{
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} enemy died!");

        // TODO: Add death animation or loot drop
        Destroy(gameObject);
    }
}
