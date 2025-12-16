using System;                     
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    [Header("Player References")]
    public PlayerControllerCombined playerController;
    public PlayerStatus playerStatus;
    public PlayerHealth playerHealth;

    [Header("Game Timer")]
    public GameTimer gameTimer;

    private int currentSlot = -1;

    // 🔥 DATA ĐANG LOAD
    private SaveData loadedData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Debug.Log("Persistent Path = " + Application.persistentDataPath);
        FindPlayerRefs();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // =========================
    // SCENE
    // =========================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerRefs();

        if (loadedData == null) return;

        // Apply enemy state
        foreach (var e in FindObjectsOfType<EnemySaveHandle>())
        {
            var match = loadedData.enemies.FirstOrDefault(x => x.id == e.enemyId);
            if (match != null)
                e.ApplyState(match);
        }

        // Apply animal state
        foreach (var a in FindObjectsOfType<AnimalSaveHandle>())
        {
            var match = loadedData.animals.FirstOrDefault(x => x.id == a.animalId);
            if (match != null)
                a.ApplyState(match);
        }
    }

    // =========================
    // FIND REFERENCES
    // =========================
    private void FindPlayerRefs()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerControllerCombined>();

        if (playerController != null)
        {
            if (playerStatus == null)
                playerStatus = playerController.GetComponent<PlayerStatus>();

            if (playerHealth == null)
                playerHealth = playerController.GetComponent<PlayerHealth>();
        }

        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();
    }

    // SAVE LOCAL ====================================
    public void SaveToSlot(int slot)
    {
        currentSlot = slot;
        FindPlayerRefs();

        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Player references missing");
            return;
        }

        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.sceneTransitionName = SceneManagement.Instance != null
        ? SceneManagement.Instance.SceneTransitionName
        : string.Empty;


        data.player = new PlayerSaveData
        {
            posX = playerController.transform.position.x,
            posY = playerController.transform.position.y,
            level = playerStatus.level,
            exp = playerStatus.exp,
            currentHP = playerStatus.CurrentHP,
            maxHP = playerStatus.MaxHP,
            expToNextLevel = playerStatus.expToNextLevel,
            damage = playerStatus.damage
        };

        data.gameTime = gameTimer != null ? gameTimer.GetTime() : 0f;

        data.enemies = FindObjectsOfType<EnemySaveHandle>(true)
            .Select(e => e.GetSaveData())
            .ToArray();

        data.animals = FindObjectsOfType<AnimalSaveHandle>(true)
            .Select(a => a.GetSaveData())
            .ToArray();

        data.playerName = PlayerPrefs.GetString("PlayerName", "Player");
        data.saveTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        SaveSystem.SaveGame(slot, data);
        Debug.Log($"[GameSaveManager] Save slot {slot} complete");
    }

    // LOAD LOCAL ====================================
    public void LoadFromSlot(int slot)
    {
        currentSlot = slot;
        loadedData = SaveSystem.LoadGame(slot);

        if (loadedData == null) return;

        StartCoroutine(LoadSceneAndApply(loadedData));
    }

    private IEnumerator LoadSceneAndApply(SaveData data)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone) yield return null;

        while (FindObjectOfType<PlayerControllerCombined>() == null)
            yield return null;

        FindPlayerRefs();

        playerController.transform.position = new Vector2(data.player.posX, data.player.posY);

        playerStatus.level = data.player.level;
        playerStatus.exp = data.player.exp;
        playerStatus.expToNextLevel = data.player.expToNextLevel;
        playerStatus.damage = data.player.damage;

        playerHealth.maxHealth = data.player.maxHP;
        playerHealth.ApplyLoadedHP(data.player.currentHP);

        playerStatus.ForceRefreshUI();

    foreach (var e in FindObjectsOfType<EnemySaveHandle>())
    {
        var match = data.enemies.FirstOrDefault(x => x.id == e.enemyId);
        if (match != null)
            e.ApplyState(match); // truyền cả object
    }

    foreach (var a in FindObjectsOfType<AnimalSaveHandle>())
    {
        var match = data.animals.FirstOrDefault(x => x.id == a.animalId);
        if (match != null)
            a.ApplyState(match); // tương tự
    }


        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.SetTime(data.gameTime);
            gameTimer.ResumeTimer();
        }

        Debug.Log($"Slot {currentSlot} loaded thành công!");
    }

    // ============================
    // SUBMIT RUN -> LEADERBOARD
    // ============================
    public void SaveCompletionTime(float completionTimeSeconds)
    {
        int timeMs = Mathf.FloorToInt(Mathf.Max(0f, completionTimeSeconds) * 1000f);

        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        string playerName = PlayerIdentity.GetPlayerName();

        Debug.Log($"[GameSaveManager] SubmitRun timeMs={timeMs}, player={playerName} ({playerId})");
        StartCoroutine(LeaderboardApi.SubmitRun(playerId, playerName, timeMs));
    }

    // ============================
    // LOAD LEADERBOARD
    // ============================
    private SaveData[] leaderboard = new SaveData[0];
    public SaveData[] GetLeaderboardData() => leaderboard;

    public void LoadLeaderboard()
    {
        StartCoroutine(LeaderboardApi.LoadLeaderboard(50, OnLeaderboardLoaded));
    }

    private void OnLeaderboardLoaded(SaveData[] dataArray)
    {
        if (dataArray == null)
        {
            Debug.LogWarning("[GameSaveManager] Leaderboard load failed (null).");
            return;
        }

        if (dataArray.Length == 0)
        {
            Debug.LogWarning("[GameSaveManager] Leaderboard empty.");
            leaderboard = new SaveData[0];
            return;
        }

        leaderboard = dataArray;

        var ui = FindObjectOfType<LeaderboardUIManager>(true);
        if (ui != null)
        {
            ui.ShowLeaderboardUI();
        }
        else
        {
            Debug.LogWarning("[GameSaveManager] NOT found LeaderboardUIManager in scene.");
        }
    }
}
