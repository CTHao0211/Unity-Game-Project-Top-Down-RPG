using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingUI;
    public GameObject saveUI;

    private bool isPaused = false;

    void Update()
    {
        // Nhấn Escape toggle Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                Pause();
        }
    }

    // Hiển thị PauseUI
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingUI.SetActive(false);
        saveUI.SetActive(false); // đảm bảo Save Panel tắt
        Time.timeScale = 0f;     // game dừng
        isPaused = true;
    }


    // Ẩn PauseUI
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Mở SettingUI từ PauseUI
    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingUI.SetActive(true);
    }


    // Thoát game

    public void QuitGame()
    {
        // Reset thời gian nếu đang pause
        Time.timeScale = 1f;

        // Load scene Main Menu (đổi tên scene đúng với project của bạn)
        SceneManager.LoadScene("StartMenu");
    }


    // Lưu game (tùy chỉnh)
    public void SaveGame()
    {
        pauseMenuUI.SetActive(false);
        saveUI.SetActive(true);
    }

}
