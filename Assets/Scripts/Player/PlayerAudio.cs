using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Movement Clips")]
    public AudioClip[] walkClips;
    public AudioClip[] runClips; // Nếu có chạy nhanh
    public AudioClip jumpClip;

    [Header("Attack Clips")]
    public AudioClip[] swordAttackClips;
    public AudioClip[] bowAttackClips;
    public AudioClip[] skillClips;

    [Header("Other Clips")]
    public AudioClip[] hitClips;
    public AudioClip deathClip;

    private void Awake()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if(clips.Length == 0 || audioSource == null) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }

    // === Movement ===
    public void PlayWalk()
    {
        PlayRandomClip(walkClips);
    }

    public void PlayRun()
    {
        PlayRandomClip(runClips);
    }

    public void PlayJump()
    {
        if(jumpClip != null)
            audioSource.PlayOneShot(jumpClip);
    }

    // === Attack ===
    public void PlaySwordAttack()
    {
        PlayRandomClip(swordAttackClips);
    }

    public void PlayBowAttack()
    {
        PlayRandomClip(bowAttackClips);
    }

    public void PlaySkill()
    {
        PlayRandomClip(skillClips);
    }

    // === Hit / Death ===
    public void PlayHit()
    {
        PlayRandomClip(hitClips);
    }

    public void PlayDeath()
    {
        if(deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }
}
