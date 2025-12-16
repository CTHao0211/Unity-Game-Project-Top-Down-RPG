using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMain;
    public GameObject panelNewGame;
    public GameObject panelLoad;
    public GameObject panelLeaderBoard;

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
        panelLeaderBoard.SetActive(false);
    }

    public void ShowNewGamePanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(true);
        panelLoad.SetActive(false);
        panelLeaderBoard.SetActive(false);
    }

    public void ShowLoadPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(true);
        panelLeaderBoard.SetActive(false);

    }

    public void ShowLeatherBoardPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(false);
        panelLeaderBoard.SetActive(true);

        GameSaveManager.Instance.LoadLeaderboard();
    }



    // ====== Nút trong Game mới ======
    public void OnClickStartGame()
    {
        string playerName = string.IsNullOrWhiteSpace(nameInput.text)
            ? "Player"
            : nameInput.text.Trim();

        PlayerIdentity.SetPlayerName(playerName);
        PlayerIdentity.GetOrCreatePlayerId(); // đảm bảo có ID

        Debug.Log("Tên người chơi: " + playerName);

        introController.StartIntro(); 
    }


    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
