using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material flashMaterial;      // Material flash trắng
    [SerializeField] private float flashDuration = 0.15f;

    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        // Tìm SpriteRenderer ở root hoặc con
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Lưu lại material gốc
        originalMaterial = spriteRenderer.material;
    }

    public IEnumerator FlashRoutine()
    {
        // Không thấy SpriteRenderer thì thôi
        if (spriteRenderer == null)
        {
            enemyHealth.DetectDeath();
            yield break;
        }

        // Gán material flash
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        // Gán trả material gốc
        spriteRenderer.material = originalMaterial;

        // Kiểm tra chết
        enemyHealth.DetectDeath();
    }
}
