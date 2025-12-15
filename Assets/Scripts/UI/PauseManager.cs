using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingUI;
    public GameObject saveUI;
    public CanvasGroup fadePanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                Pause();
        }
    }

    public void Pause()
    {
        fadePanel.alpha = 0.5f;
        fadePanel.blocksRaycasts = true;
        
        pauseMenuUI.SetActive(true);
        settingUI.SetActive(false);
        saveUI.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        fadePanel.alpha = 0;
        fadePanel.blocksRaycasts = false;

        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingUI.SetActive(true);
    }

    public void SaveGame()
    {
        pauseMenuUI.SetActive(false);
        saveUI.SetActive(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }
}
