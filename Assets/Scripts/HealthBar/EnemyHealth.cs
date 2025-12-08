using UnityEngine;

public class EnemyHealth : HealthBase
{
    [SerializeField] private GameObject deathVFXPrefab;
    public override void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        base.TakeDamage(dmg, source, popupColor);

        if (currentHealth <= 0)
        {
            DieWithVFX();
        }
    }

    private void DieWithVFX()
    {
        if (deathVFXPrefab != null)
        {
            GameObject vfx = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Debug.Log($"[HealthBase] Spawn VFX: {vfx.name} tại {vfx.transform.position}");
        }

        Die(); // gọi logic Die gốc (âm thanh, destroy enemy)
    }

    protected override void Die()
    {
        AnimalAudio audio = GetComponent<AnimalAudio>();
        if (audio != null)
            audio.PlayDeathSound();

        Destroy(gameObject, 0.1f);
    }
}
