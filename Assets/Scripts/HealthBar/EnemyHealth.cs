using UnityEngine;
using System.Collections;

public class EnemyHealth : HealthBase
{
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float vfxLifetime = 1.5f;
    public int expReward = 2; // EXP cấp cho player khi chết

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
            Destroy(vfx, vfxLifetime);
        }

        // Cấp EXP cho player
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
        {
            player.AddExp(expReward);
        }

        Die(); // gọi logic Die gốc (âm thanh, disable)
    }

    protected override void Die()
    {
        AnimalAudio audio = GetComponent<AnimalAudio>();
        if (audio != null)
            audio.PlayDeathSound();

        // Tắt thay vì phá hủy
        StartCoroutine(DisableAfterDelay(0.3f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); 
    }
}
