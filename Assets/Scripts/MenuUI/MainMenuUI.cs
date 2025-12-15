using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMain;    // cái panel có START GAME, Game mới, Load, Cài đặt, Thoát
    public GameObject panelNewGame; // panel đặt tên
    public GameObject panelLoad;    // panel load slot
    public GameObject panelSetting; // nếu chưa có thì để trống

    [Header("New Game")]
    public TMP_InputField nameInput;
    public string gameplaySceneName = "Scene1";   // Đổi đúng tên scene chơi game của bạn

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
    }



    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
