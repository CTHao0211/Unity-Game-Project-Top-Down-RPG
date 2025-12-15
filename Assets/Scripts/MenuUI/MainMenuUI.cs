using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMain;
    public GameObject panelNewGame;
    public GameObject panelLoad;
    public GameObject panelSetting;

    [Header("New Game")]
    public TMP_InputField nameInput;

    [Header("Intro")]
    public IntroController introController;   // ⭐ thêm dòng này

    private void Start()
    {
        ShowMainPanel();
    }

    // ====== Chuyển panel ======
    public void ShowMainPanel()
    {
        panelMain.SetActive(true);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(false);
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void ShowNewGamePanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(true);
        panelLoad.SetActive(false);
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void ShowLoadPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(true);
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void ShowSettingPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(false);
        if (panelSetting != null) panelSetting.SetActive(true);
    }

    // ====== Nút trong Game mới ======
    public void OnClickStartGame()
    {
        string playerName = string.IsNullOrWhiteSpace(nameInput.text)
            ? "Player"
            : nameInput.text.Trim();

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        Debug.Log("Tên người chơi: " + playerName);

        PlayerIdentity.SetPlayerName(playerName);
        SceneManager.LoadScene(gameplaySceneName);
        // ⭐ KHÔNG load scene ở đây
        introController.StartIntro(); // 👉 chạy intro
    }

    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
