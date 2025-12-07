using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingUI;

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
        settingUI.SetActive(false);   // đảm bảo SettingUI tắt
        Time.timeScale = 0f;
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

    // Đóng SettingUI và trở về PauseUI
    public void CloseSettings()
    {
        settingUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    // Thoát game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Lưu game (tùy chỉnh)
    public void SaveGame()
    {
        Debug.Log("Game Saved!");
        // Thêm logic lưu game tại đây
    }
}
