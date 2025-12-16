using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text playerNameText;
    public TMP_Text survivalTimeText;
    public TMP_Text killCountText;

    [Header("Game Win UI")]
    public GameObject gameWinPanel;
    public TMP_Text winPlayerNameText;
    public TMP_Text winTimeText;
    public TMP_Text winKillText;

    [Header("Player Info")]
    public string playerName = "Player";

    [Header("Stats")]
    public float survivalTime = 0f;
    public int killCount = 0;

    public bool IsGameOver { get; private set; } = false;
    public bool IsGameWin { get; private set; } = false;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            playerName = PlayerPrefs.GetString("PlayerName", "Player");

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeRoutine(0f));
        }

        ResetGameState();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (IsGameOver || IsGameWin) return;
        survivalTime += Time.deltaTime;
    }

    public void AddKill()
    {
        if (IsGameOver || IsGameWin) return;
        killCount++;
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (playerNameText != null) playerNameText.text = "Player: " + playerName;
        if (survivalTimeText != null) survivalTimeText.text = "Time: " + FormatTime(survivalTime);
        if (killCountText != null) killCountText.text = "Kills: " + killCount;

        Time.timeScale = 0f;
    }

    public void GameWin()
    {
        Debug.Log(">>> GameWin() CALLED <<<");
        if (IsGameWin || IsGameOver) return;

        IsGameWin = true;

        if (gameWinPanel != null) gameWinPanel.SetActive(true);
        if (winPlayerNameText != null) winPlayerNameText.text = "Player: " + playerName;
        if (winTimeText != null) winTimeText.text = "Time: " + FormatTime(survivalTime);
        if (winKillText != null) winKillText.text = "Kills: " + killCount;
        TriggerWin();
        Time.timeScale = 0f;
    }
    public void TriggerWin()
    {
        var gsm = GameSaveManager.Instance;
        if (gsm == null) return;

        float timeSec = (gsm.gameTimer != null) ? gsm.gameTimer.GetTime() : 0f;
        int timeMs = Mathf.FloorToInt(timeSec * 1000f);

        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        string playerName = PlayerIdentity.GetPlayerName();

        gsm.StartCoroutine(LeaderboardApi.SubmitRun(playerId, playerName, timeMs));
    }
    private string FormatTime(float time)
    {
        int total = Mathf.FloorToInt(time);
        int minutes = total / 60;
        int seconds = total % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    public void ResetGameState()
    {
        IsGameOver = false;
        IsGameWin = false;

        survivalTime = 0f;
        killCount = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        if (playerNameText != null) playerNameText.text = "";
        if (survivalTimeText != null) survivalTimeText.text = "";
        if (killCountText != null) killCountText.text = "";

        if (winPlayerNameText != null) winPlayerNameText.text = "";
        if (winTimeText != null) winTimeText.text = "";
        if (winKillText != null) winKillText.text = "";
    }

    public void OnClickRestart()
    {
        if (isTransitioning) return;
        StartCoroutine(RestartRoutine());
    }

    public void OnClickReturnToMenu()
    {
        if (isTransitioning) return;
        StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        // Fade out
        yield return StartCoroutine(FadeRoutine(1f));

        // Load lại Scene game
        SceneManager.LoadScene("Scene1");

        yield return null;

        isTransitioning = false;
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        // Fade out
        yield return StartCoroutine(FadeRoutine(1f));

        // Load Menu
        SceneManager.LoadScene("StartMenu");

        yield return null;

        isTransitioning = false;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
