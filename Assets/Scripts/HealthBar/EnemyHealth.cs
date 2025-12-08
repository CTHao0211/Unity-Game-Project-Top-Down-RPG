using UnityEngine;

public class EnemyHealth : HealthBase
{
    protected override void Die()
    {
        AnimalAudio audio = GetComponent<AnimalAudio>();
        if(audio != null)
            audio.PlayDeathSound(); // phát deathClip

        // Các logic chết khác
        Destroy(gameObject, 0.1f); // delay nhỏ để nghe âm thanh
    }

}
