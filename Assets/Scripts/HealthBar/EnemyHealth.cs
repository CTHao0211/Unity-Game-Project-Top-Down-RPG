using UnityEngine;
using System.Collections;

public class EnemyHealth : HealthBase
{
    [Header("Death VFX")]
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float vfxLifetime = 1.5f;

    [Header("Rewards")]
    public int expReward = 2; // EXP cấp cho player khi chết

    [Header("Boss Settings")]
    [SerializeField] private bool isBoss = false; // ✅ tick true cho BossSlime

    private bool hasDied = false; // ✅ tránh chết nhiều lần

    public override void TakeDamage(int dmg, Transform source = null, Color? popupColor = null)
    {
        if (hasDied) return;

        base.TakeDamage(dmg, source, popupColor);

        if (currentHealth <= 0)
        {
            hasDied = true;
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

        // ✅ Cấp EXP cho player
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
        {
            player.AddExp(expReward);
        }

        // ✅ Nếu là boss => WIN
        if (isBoss)
        {
            GameManager.instance?.GameWin();
        }
        else
        {
            // ✅ Quái thường => cộng kill
            GameManager.instance?.AddKill();
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
