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

    [Header("Game Timer")]
    public GameTimer gameTimer;

    [Header("Auto Save")]
    public float autoSaveInterval = 600f;

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

        if (SaveSystem.HasSave())
        {
            Debug.Log("[GameSaveManager] Found save file, auto loading...");
            Load();
        }
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

        if (playerController != null && playerStatus == null)
            playerStatus = playerController.GetComponent<PlayerStatus>();

        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();
    }

    // AUTO SAVE ====================================
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            Save();
            Debug.Log("[AutoSave] Saved.");
        }
    }

    // SAVE ==========================================
    public void Save()
    {
        Debug.Log("[GameSaveManager] Save() called.");
        FindPlayerRefs();

        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Missing player references.");
            return;
        }

        SaveData data = new SaveData();

        // Scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // Player
        data.player = new PlayerSaveData();
        data.player.posX = playerController.transform.position.x;
        data.player.posY = playerController.transform.position.y;

        data.player.level = playerStatus.level;
        data.player.exp = playerStatus.exp;
        data.player.currentHP = playerStatus.CurrentHP;
        data.player.maxHP = playerStatus.MaxHP;

        // GameTime
        data.gameTime = gameTimer != null ? gameTimer.GetTime() : 0f;

        // Enemies
        var enemies = FindObjectsOfType<EnemySaveHandle>(true);
        data.enemies = enemies.Select(e => new EnemySaveData
        {
            id = e.enemyId,
            posX = e.transform.position.x,
            posY = e.transform.position.y,
            currentHP = e.CurrentHP,
            isDead = e.IsDead
        }).ToArray();

        // Animals
        var animals = FindObjectsOfType<AnimalSaveHandle>(true);
        data.animals = animals.Select(a => new AnimalSaveData
        {
            id = a.animalId,
            posX = a.transform.position.x,
            posY = a.transform.position.y,
            currentHP = a.CurrentHP,
            isDead = a.IsDead
        }).ToArray();

        // SAVE FILE
        SaveSystem.SaveGame(data);
        Debug.Log("[GameSaveManager] Save complete.");
    }

    // LOAD ==========================================
    public void Load()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("[GameSaveManager] No save file.");
            return;
        }

        StartCoroutine(LoadSceneAndApply(data));
    }

    private IEnumerator LoadSceneAndApply(SaveData data)
    {
        // Load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone) yield return null;

        // Find references after scene load
        FindPlayerRefs();

        // Restore gameTime
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.ResumeTimer();

            typeof(GameTimer)
                .GetField("elapsedTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(gameTimer, data.gameTime);
        }

        // Player
        playerController.transform.position = new Vector3(data.player.posX, data.player.posY, 0);
        playerStatus.level = data.player.level;
        playerStatus.exp = data.player.exp;
        playerStatus.MaxHP = data.player.maxHP;
        playerStatus.CurrentHP = data.player.currentHP;

        // Enemies
        foreach (var e in FindObjectsOfType<EnemySaveHandle>())
        {
            var match = data.enemies.FirstOrDefault(x => x.id == e.enemyId);
            if (match == null) continue;

            e.ApplyState(match.posX, match.posY, match.currentHP, match.isDead);
        }

        foreach (var a in FindObjectsOfType<AnimalSaveHandle>())
        {
            var match = data.animals.FirstOrDefault(x => x.id == a.animalId);
            if (match == null) continue;

            a.ApplyState(match.posX, match.posY, match.currentHP, match.isDead);
        }


        Debug.Log("[GameSaveManager] Load complete.");
    }

    // TEST KEY ======================================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyDown(KeyCode.F9)) Load();
    }
}
