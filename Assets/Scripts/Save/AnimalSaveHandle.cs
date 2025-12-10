using UnityEngine;
using System;

public class AnimalSaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string animalId;

    [Header("Health (HealthBase) - nếu có")]
    public HealthBase health;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBase>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(animalId))
        {
            animalId = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        if (health == null)
            health = GetComponent<HealthBase>();
    }

    public int CurrentHP
    {
        get => health != null ? health.currentHealth : 0;
        set
        {
            if (health == null) return;
            health.currentHealth = Mathf.Clamp(value, 0, health.maxHealth);
        }
    }
    public bool IsDead
    {
        get
        {
            if (!gameObject.activeSelf) return true;
            if (health == null) return false;
            return health.currentHealth <= 0;
        }
    }
    public bool HasHealth => health != null;

    public Vector3 GetPosition() => transform.position;

    public bool GetFlipX() => spriteRenderer != null && spriteRenderer.flipX;

    public void ApplyState(float x, float y, bool flipX, int? hp = null)
    {
        transform.position = new Vector3(x, y, transform.position.z);

        if (spriteRenderer != null)
            spriteRenderer.flipX = flipX;

        if (hp.HasValue && health != null)
            CurrentHP = hp.Value;
    }
}
