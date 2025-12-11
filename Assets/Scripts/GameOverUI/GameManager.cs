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

    [Header("Player Info")]
    public string playerName = "Player";

    [Header("Stats")]
    public float survivalTime = 0f;
    public int killCount = 0;
    public bool IsGameOver { get; private set; } = false;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;   // Panel đen full màn
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
        // Mỗi scene đều có 1 GameManager riêng => KHÔNG cần DontDestroyOnLoad
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 🔹 LẤY TÊN NGƯỜI CHƠI ĐÃ LƯU TỪ STARTMENU
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName", "Player");
        }

        // Fade in khi vào scene
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeRoutine(0f)); // fade về trong suốt
        }

        // Ẩn Game Over lúc đầu
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }


    private void Update()
    {
        if (IsGameOver) return;
        survivalTime += Time.deltaTime;
    }

    // Gọi khi quái chết
    public void AddKill()
    {
        if (IsGameOver) return;
        killCount++;
    }

    // Gọi khi player chết
    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (playerNameText != null)
            playerNameText.text = "Player: " + playerName;

        if (survivalTimeText != null)
            survivalTimeText.text = "Time: " + FormatTime(survivalTime);

        if (killCountText != null)
            killCountText.text = "Kills: " + killCount;

        Time.timeScale = 0f; // dừng game (UI vẫn chạy)
    }

    private string FormatTime(float time)
    {
        int total = Mathf.FloorToInt(time);
        int minutes = total / 60;
        int seconds = total % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    // ========= BUTTON CALLBACKS =========

    // Gắn vào nút "Chơi lại"
    public void OnClickRestart()
    {
        if (isTransitioning) return;
        StartCoroutine(RestartRoutine());
    }

    // Gắn vào nút "Trở về"
    public void OnClickReturnToMenu()
    {
        if (isTransitioning) return;
        StartCoroutine(ReturnToMenuRoutine());
    }

    // ========= FADE + LOAD SCENE =========

    private IEnumerator RestartRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 1f; // bật lại thời gian để fade hoạt động

        yield return StartCoroutine(FadeRoutine(1f)); // fade đen

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        yield return StartCoroutine(FadeRoutine(1f)); // fade đen

        SceneManager.LoadScene("StartMenu"); // ĐỔI đúng tên scene StartMenu của bạn
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
