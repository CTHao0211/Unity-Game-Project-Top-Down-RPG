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
        // ✅ an toàn: nếu có nhiều GameManager thì giữ cái đầu tiên
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        // ✅ nếu vì lý do nào đó instance bị mất, tự set lại
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            playerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Fade in khi vào scene
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeRoutine(0f));
        }

        // Ẩn panel lúc đầu
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        Time.timeScale = 1f;

        // reset state mỗi scene
        IsGameOver = false;
        IsGameWin = false;
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

    // ✅ GameOver luôn ưu tiên chạy khi player chết
    public void GameOver()
    {
        if (IsGameOver) return;   // ❗ không chặn bởi IsGameWin nữa
        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogWarning("[GameManager] gameOverPanel is NULL");

        if (playerNameText != null)
            playerNameText.text = "Player: " + playerName;

        if (survivalTimeText != null)
            survivalTimeText.text = "Time: " + FormatTime(survivalTime);

        if (killCountText != null)
            killCountText.text = "Kills: " + killCount;

        Time.timeScale = 0f;
    }

    // ✅ Win chỉ xảy ra ở Scene2 (đúng yêu cầu)
    public void GameWin()
    {
        if (IsGameWin || IsGameOver) return;

        // ❗ Chỉ win ở Scene2
        if (SceneManager.GetActiveScene().name != "Scene2")
        {
            Debug.Log("[GameManager] GameWin ignored because not in Scene2");
            return;
        }

        IsGameWin = true;

        if (gameWinPanel != null)
            gameWinPanel.SetActive(true);
        else
            Debug.LogWarning("[GameManager] gameWinPanel is NULL");

        if (winPlayerNameText != null)
            winPlayerNameText.text = "Player: " + playerName;

        if (winTimeText != null)
            winTimeText.text = "Time: " + FormatTime(survivalTime);

        if (winKillText != null)
            winKillText.text = "Kills: " + killCount;

        Time.timeScale = 0f;
    }

    private string FormatTime(float time)
    {
        int total = Mathf.FloorToInt(time);
        int minutes = total / 60;
        int seconds = total % 60;
        return $"{minutes:00}:{seconds:00}";
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

        yield return StartCoroutine(FadeRoutine(1f));

        SceneManager.LoadScene("Scene1");
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        yield return StartCoroutine(FadeRoutine(1f));

        SceneManager.LoadScene("StartMenu");
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;

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
