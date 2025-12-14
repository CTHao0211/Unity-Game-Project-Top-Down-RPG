using UnityEngine;
using System;

public class AnimalSaveHandle : MonoBehaviour
{
    [Header("Stable ID (không đổi)")]
    public string animalId;

    [Header("Optional")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private HealthBase health;

    public bool HasHealth => health != null;

    public int CurrentHP
    {
        get => health != null ? health.currentHealth : 0;
        set
        {
            if (health != null) health.currentHealth = Mathf.Max(0, value);
        }
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        health = GetComponent<HealthBase>();
    }

    public bool GetFlipX()
    {
        return spriteRenderer != null && spriteRenderer.flipX;
    }

    public void ApplyState(float x, float y, bool flipX, int? hp)
    {
        transform.position = new Vector3(x, y, transform.position.z);

        if (spriteRenderer != null)
            spriteRenderer.flipX = flipX;

        if (health != null && hp.HasValue)
            health.currentHealth = Mathf.Max(0, hp.Value);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(animalId))
            animalId = Guid.NewGuid().ToString("N");

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    [ContextMenu("Regenerate ID")]
    private void RegenerateId()
    {
        animalId = Guid.NewGuid().ToString("N");
    }
#endif
}
