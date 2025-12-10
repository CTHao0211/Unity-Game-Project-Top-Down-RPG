using UnityEngine;

public class AnimalAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip[] randomIdleClips;
    public AudioClip[] hitClips;
    public AudioClip[] eventClips;
    public AudioClip deathClip;
    
    [Header("Special Clips")]
    public AudioClip[] specialClips; // âm thanh riêng cho enemy đặc biệt

    [Header("Movement Clips")]
    public AudioClip[] movementClips;     // tiếng bước chân, tiếng xương
    public float movementInterval = 0.4f; // tốc độ phát khi di chuyển

    private float nextMoveSoundTime;

    [Header("Random Idle Settings")]
    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private float nextIdleTime;

    private void Start()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ScheduleNextIdle();
    }

    private void Update()
    {
        if(randomIdleClips.Length > 0 && Time.time >= nextIdleTime)
        {
            PlayRandomClip(randomIdleClips);
            ScheduleNextIdle();
        }
    }
    public void PlayMovementSound(float speed)
    {
        if (movementClips.Length == 0) return;

        // chỉ phát khi đang di chuyển
        if (speed > 0.05f && Time.time >= nextMoveSoundTime)
        {
            PlayRandomClip(movementClips);
            nextMoveSoundTime = Time.time + movementInterval;
        }
    }

    private void ScheduleNextIdle()
    {
        nextIdleTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if(clips.Length == 0 || audioSource == null) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }

    public void PlayHitSound()
    {
        if(hitClips.Length > 0)
            PlayRandomClip(hitClips);
    }

    public void PlayEventSound()
    {
        if(eventClips.Length > 0)
            PlayRandomClip(eventClips);
    }

    public void PlayDeathSound()
    {
        if(deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }

    // PHÁT ÂM THANH ĐẶC BIỆT
    public void PlaySpecialSound()
    {
        if(specialClips.Length > 0)
            PlayRandomClip(specialClips);
    }
}
