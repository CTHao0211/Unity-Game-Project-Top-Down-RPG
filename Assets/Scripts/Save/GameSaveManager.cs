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

    // =========================
    // LIFECYCLE
    // =========================
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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayerRefs();
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

    // =========================
    // SAVE
    // =========================
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

        // Game time
        data.gameTime = gameTimer != null ? gameTimer.GetTime() : 0f;

        // Enemies
        data.enemies = FindObjectsOfType<EnemySaveHandle>(true)
            .Select(e => e.GetSaveData())
            .ToArray();

        // Animals
        data.animals = FindObjectsOfType<AnimalSaveHandle>(true)
            .Select(a => a.GetSaveData())
            .ToArray();

        // Metadata
        data.playerName = PlayerPrefs.GetString("PlayerName", "Player");
        data.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        SaveSystem.SaveGame(slot, data);
        Debug.Log($"[GameSaveManager] Save slot {slot} complete");
    }

    // =========================
    // LOAD
    // =========================
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

        // 🔥 RESTORE TRANSITION
        if (SceneManagement.Instance != null)
        {
            SceneManagement.Instance.SetTransitionName(data.sceneTransitionName);
        }

        // Chờ player spawn
        while (FindObjectOfType<PlayerControllerCombined>() == null)
            yield return null;

        FindPlayerRefs();

        // 🔥 KHÔNG GHI ĐÈ VỊ TRÍ NẾU CÓ TRANSITION
        if (string.IsNullOrEmpty(data.sceneTransitionName))
        {
            playerController.transform.position =
                new Vector2(data.player.posX, data.player.posY);
        }

        // Player stats
        playerStatus.level = data.player.level;
        playerStatus.exp = data.player.exp;
        playerStatus.expToNextLevel = data.player.expToNextLevel;
        playerStatus.damage = data.player.damage;

        playerHealth.maxHealth = data.player.maxHP;
        playerHealth.ApplyLoadedHP(data.player.currentHP);

        playerStatus.ForceRefreshUI();

        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.SetTime(data.gameTime);
            gameTimer.ResumeTimer();
        }
    }

}
