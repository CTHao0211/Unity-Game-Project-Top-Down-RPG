using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMain;
    public GameObject panelNewGame;
    public GameObject panelLoad;
    public GameObject panelLeaderBoard;

    [Header("New Game")]
    public TMP_InputField nameInput;
    public TextMeshProUGUI txtError;

    [Header("Intro")]
    public IntroController introController;

    private void Start()
    {
        ShowMainPanel();
        HideError();
    }

    public void ShowMainPanel()
    {
        panelMain.SetActive(true);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(false);
        panelLeaderBoard.SetActive(false);
        HideError();
    }

    public void ShowNewGamePanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(true);
        panelLoad.SetActive(false);
        panelLeaderBoard.SetActive(false);
        HideError();
    }

    public void ShowLoadPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(true);
        panelLeaderBoard.SetActive(false);
        HideError();
    }

    public void ShowLeatherBoardPanel()
    {
        panelMain.SetActive(false);
        panelNewGame.SetActive(false);
        panelLoad.SetActive(false);
        panelLeaderBoard.SetActive(true);
        HideError();

        GameSaveManager.Instance.LoadLeaderboard();
    }

    private void ShowError(string msg)
    {
        Debug.Log($"[DEBUG] Đang gọi ShowError với nội dung: '{msg}'"); 

        if (txtError == null)
        {
            Debug.LogError("[DEBUG] LỖI: Biến txtError đang bị NULL! (Chưa kéo vào Inspector)");
            return;
        }

        txtError.text = msg;
        txtError.gameObject.SetActive(true);

        Debug.Log($"[DEBUG] Đã bật txtError. Vị trí trên màn hình: {txtError.rectTransform.position}");
    }

    private void HideError()
    {
        if (txtError == null) return;
        txtError.gameObject.SetActive(false);
    }

    public void OnNameEndEdit(string value)
    {
        HideError();

        string name = string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        if (name.Length == 0) return;

        StartCoroutine(AuthApi.CheckNameAvailable(name, (resp) =>
        {
            if (resp == null || !resp.ok) return; 

            if (resp.available)
            {
                HideError();
            }
            else
            {
                string playerId = PlayerIdentity.GetOrCreatePlayerId();

                StartCoroutine(AuthApi.Login(playerId, name, (loginResp) =>
                {
                    if (loginResp != null && loginResp.ok)
                    {
                        HideError();
                    }
                    else
                    {
                        ShowError("Tên đã tồn tại.");
                    }
                }));
            }
        }));
    }

    public void OnClickStartGame()
    {
        HideError();

        string playerName = string.IsNullOrWhiteSpace(nameInput?.text) ? "" : nameInput.text.Trim();
        if (playerName.Length == 0)
        {
            ShowError("Vui lòng nhập tên.");
            return;
        }

        string playerId = PlayerIdentity.GetOrCreatePlayerId(); 

        StartCoroutine(AuthApi.Register(playerId, playerName, (resp) =>
        {
            if (resp == null)
            {
                ShowError("Lỗi kết nối. Vui lòng thử lại.");
                return;
            }

            if (resp.ok)
            {
                EnterGame(playerName);
                return;
            }

            if (resp.error == "name_taken")
            {
                StartCoroutine(AuthApi.Login(playerId, playerName, (loginResp) =>
                {
                    if (loginResp != null && loginResp.ok)
                    {
                        Debug.Log("Đăng nhập thành công lại vào nick cũ!");
                        EnterGame(playerName);
                    }
                    else
                    {
                        ShowError("Tên đã tồn tại.");
                    }
                }));
                return;
            }

            ShowError("Lỗi không xác định: " + resp.error);
        }));
    }

    private void EnterGame(string validName)
    {
        HideError();

        PlayerIdentity.SetPlayerName(validName);
        PlayerPrefs.SetString("PlayerName", validName);
        PlayerPrefs.Save();


        if (introController != null)
            introController.StartIntro();
    }

    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}
