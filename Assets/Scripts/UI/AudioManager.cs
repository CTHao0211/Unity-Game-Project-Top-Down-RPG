using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;      // Nhạc nền
    public AudioSource sfxSource;        // Hiệu ứng

    [Header("UI")]
    public Button muteButton;            // Nút UI
    public Sprite soundOnIcon;           // Icon khi âm thanh bật
    public Sprite soundOffIcon;          // Icon khi âm thanh tắt

    private bool isMuted = false;

    void Awake()
    {
        // Lấy trạng thái mute từ lần chơi trước
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;

        // Cập nhật AudioSource và icon theo trạng thái
        ApplyMute();
    }

    void Start()
    {
        if (muteButton != null)
        {
            // Gán sự kiện click
            muteButton.onClick.AddListener(ToggleMute);
        }

        // Đảm bảo nhạc nền loop
        if (musicSource != null)
        {
            musicSource.loop = true;
            if (!musicSource.isPlaying && !isMuted)
                musicSource.Play();
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        ApplyMute();

        // Lưu trạng thái
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyMute()
    {
        if (musicSource != null) musicSource.mute = isMuted;
        if (sfxSource != null) sfxSource.mute = isMuted;

        // Cập nhật icon
        if (muteButton != null)
        {
            Image img = muteButton.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = isMuted ? soundOffIcon : soundOnIcon;
            }
        }
    }

    // Tùy chọn: phát SFX
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
