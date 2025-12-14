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
    public GameTimer gameTimer; // thêm này nếu có

    [Header("Auto Save")]
    public float autoSaveInterval = 600f;
    public bool enableAutoSave = true;

    [Header("Cloud")]
    public string cloudBaseUrl = "https://cloud-save-server.onrender.com/api";
    public bool autoLoadOnStart = false;

    private int currentSlot = -1; // thêm biến slot hiện tại
    
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


    private void Start()
    {
        Debug.Log("Persistent Path = " + Application.persistentDataPath);

        FindPlayerRefs();
        StartCoroutine(AutoSaveRoutine());
    }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerRefs();
    }

    private void FindPlayerRefs()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerControllerCombined>();

        if (playerController != null)
        {
            if (playerStatus == null)
                playerStatus = playerController.GetComponent<PlayerStatus>();

            if (playerHealth == null)
                playerHealth = playerController.GetComponent<PlayerHealth>(); // ✅
        }

        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();
    }


    // AUTO SAVE ====================================
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);

            if (currentSlot >= 0)
            {
                SaveToSlot(currentSlot);
                Debug.Log("[AutoSave] Saved slot " + currentSlot);
            }
        }
    }


    // SAVE ==========================================
    public void SaveToSlot(int slot)
    {
        Debug.Log($"[GameSaveManager] Save slot {slot}");
        FindPlayerRefs();

        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Missing player references.");
            return;
        }

        SaveData data = new SaveData();

        data.sceneName = SceneManager.GetActiveScene().name;

        // Player
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

        // GameTime
        data.gameTime = gameTimer != null ? gameTimer.GetTime() : 0f;

        // Enemies
        data.enemies = FindObjectsOfType<EnemySaveHandle>(true)
            .Select(e => new EnemySaveData
            {
                id = e.enemyId,
                posX = e.transform.position.x,
                posY = e.transform.position.y,
                currentHP = e.CurrentHP,
                isDead = e.IsDead
            }).ToArray();

        // Animals
        data.animals = FindObjectsOfType<AnimalSaveHandle>(true)
            .Select(a => new AnimalSaveData
            {
                id = a.animalId,
                posX = a.transform.position.x,
                posY = a.transform.position.y,
                currentHP = a.CurrentHP,
                isDead = a.IsDead
            }).ToArray();

        // ✅ METADATA CHO SAVE UI (ĐẶT Ở ĐÂY)
        data.playerName = PlayerPrefs.GetString("PlayerName", "Player");
        data.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        SaveSystem.SaveGame(slot, data);
        Debug.Log($"[GameSaveManager] Save slot {slot} complete");
    }


    // LOAD ==========================================
    public void LoadFromSlot(int slot)
    {
        currentSlot = slot;

        SaveData data = SaveSystem.LoadGame(slot);
        if (data == null) return;

        StartCoroutine(LoadSceneAndApply(data));
    }



private IEnumerator LoadSceneAndApply(SaveData data)
{
    // Load scene
    AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
    while (!op.isDone) yield return null;

    // Chờ player được spawn
    while (FindObjectOfType<PlayerControllerCombined>() == null)
        yield return null;

    FindPlayerRefs();

    // Áp dụng dữ liệu player
    // Vị trí
    playerController.transform.position = new Vector2(data.player.posX, data.player.posY);
    // Player stats
    playerStatus.level = data.player.level;
    playerStatus.exp = data.player.exp;
    playerStatus.expToNextLevel = data.player.expToNextLevel;
    playerStatus.damage = data.player.damage;

    // Player HP (QUAN TRỌNG THỨ TỰ)
    playerHealth.maxHealth = data.player.maxHP;
    playerHealth.ApplyLoadedHP(data.player.currentHP); // ✅

    // UI
    playerStatus.ForceRefreshUI();


    // Áp dụng enemy
    foreach (var e in FindObjectsOfType<EnemySaveHandle>())
    {
        var match = data.enemies.FirstOrDefault(x => x.id == e.enemyId);
        if (match != null)
            e.ApplyState(match.posX, match.posY, match.currentHP, match.isDead);
    }

    // Áp dụng animal
    foreach (var a in FindObjectsOfType<AnimalSaveHandle>())
    {
        var match = data.animals.FirstOrDefault(x => x.id == a.animalId);
        if (match != null)
            a.ApplyState(match.posX, match.posY, match.currentHP, match.isDead);
    }

    // Khôi phục thời gian chơi
    if (gameTimer != null)
    {
        gameTimer.ResetTimer();
        gameTimer.SetTime(data.gameTime);
        gameTimer.ResumeTimer();
    }

    Debug.Log($"Slot {currentSlot} loaded thành công!");
}
// ============================
// SAVE (cloud metadata only)
// ============================
    public void SaveCompletionTime(float completionTime)
    {
        SaveData data = new SaveData();
        data.playerName = PlayerIdentity.GetPlayerName();
        data.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        data.completionTime = completionTime;

        string playerId = PlayerIdentity.GetOrCreatePlayerId();

        Debug.Log("[CloudSave] Sending completion time: " + JsonUtility.ToJson(data));
        StartCoroutine(CloudSaveApi.SaveAll(playerId, data.playerName, data));
    }


// ============================
// LOAD (cloud metadata only)
// ============================
    SaveData[] leaderboard;
    public void LoadLeaderboard()
    {
        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        StartCoroutine(CloudSaveApi.LoadAll(playerId, OnLeaderboardLoaded));
    }

    private void OnLeaderboardLoaded(SaveData[] dataArray)
    {
        if (dataArray == null || dataArray.Length == 0)
        {
            Debug.LogWarning("[GameSaveManager] No cloud leaderboard found.");
            return;
        }

        foreach (var entry in dataArray)
        {
            Debug.Log($"Leaderboard entry: {entry.playerName} - {entry.completionTime}s");
        }

        // Update UI leaderboard ở đây
    }



}
