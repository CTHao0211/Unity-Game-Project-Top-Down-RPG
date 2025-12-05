using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material flashMaterial;      
    [SerializeField] private float flashDuration = 0.15f;

    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        // Tìm SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Lưu material gốc
        originalMaterial = spriteRenderer.sharedMaterial;

        // Clone flash material để mỗi enemy dùng 1 bản riêng
        flashMaterial = new Material(flashMaterial);
    }

    public IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null)
        {
            enemyHealth.DetectDeath();
            yield break;
        }

        // Bật flash
        spriteRenderer.sharedMaterial = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        // Trả về material gốc
        spriteRenderer.sharedMaterial = originalMaterial;

        enemyHealth.DetectDeath();
    }
}
