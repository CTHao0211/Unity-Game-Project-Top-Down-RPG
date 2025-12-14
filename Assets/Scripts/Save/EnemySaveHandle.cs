using UnityEngine;
using System;

public class EnemySaveHandle : MonoBehaviour
{
    [Header("Stable ID (không đổi)")]
    public string enemyId;

    private HealthBase health;

    public int CurrentHP
    {
        get
        {
            if (health == null) health = GetComponent<HealthBase>();
            return health != null ? health.currentHealth : 0;
        }
        set
        {
            if (health == null) health = GetComponent<HealthBase>();
            if (health != null) health.currentHealth = Mathf.Max(0, value);
        }
    }

    public bool IsDead
    {
        get
        {
            if (!gameObject.activeSelf) return true;
            if (health == null) health = GetComponent<HealthBase>();
            return (health != null && health.currentHealth <= 0);
        }
    }

    private void Awake()
    {
        health = GetComponent<HealthBase>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // chỉ tạo mới nếu rỗng
        if (string.IsNullOrEmpty(enemyId))
            enemyId = Guid.NewGuid().ToString("N");
    }

    [ContextMenu("Regenerate ID")]
    private void RegenerateId()
    {
        enemyId = Guid.NewGuid().ToString("N");
    }
#endif
}
