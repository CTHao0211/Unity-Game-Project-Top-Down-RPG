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

    [Header("Auto Save")]
    public float autoSaveInterval = 600f; // 10 phút
    public bool enableAutoSave = true;

    [Header("Cloud")]
    public string cloudBaseUrl = "https://cloud-save-server.onrender.com/api";
    public bool autoLoadOnStart = false; // muốn “vào game là load luôn” thì bật

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // QUAN TRỌNG: object này phải là ROOT trong Hierarchy
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // set url cho CloudSaveApi
        CloudSaveApi.BASE_URL = cloudBaseUrl;

        // đảm bảo có identity
        PlayerIdentity.GetOrCreatePlayerId();
        if (string.IsNullOrEmpty(PlayerIdentity.GetPlayerName()))
            PlayerIdentity.SetPlayerName("Guest");

        Debug.Log("[GameSaveManager] playerId=" + PlayerIdentity.GetOrCreatePlayerId()
            + " name=" + PlayerIdentity.GetPlayerName());

        FindPlayerRefs();

        if (enableAutoSave)
            StartCoroutine(AutoSaveRoutine());

        if (autoLoadOnStart)
            Load();
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
    }

    // ============================
    // AUTO SAVE LOOP
    // ============================
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            Save();
            Debug.Log("[AutoSave] Saved to cloud.");
        }
    }

    // ============================
    // BUILD SAVE DATA (current scene)
    // ============================
    private SaveData BuildSaveDataForCurrentScene()
    {
        SaveData data = new SaveData();

        // 1) Scene
        Scene currentScene = SceneManager.GetActiveScene();
        data.sceneName = currentScene.name;

        // 2) Player
        data.player = new PlayerSaveData();
        Vector3 pPos = playerController.transform.position;
        data.player.posX = pPos.x;
        data.player.posY = pPos.y;

        data.player.level = playerStatus.level;
        data.player.exp = playerStatus.exp;
        data.player.currentHP = playerStatus.CurrentHP;
        data.player.maxHP = playerStatus.MaxHP;

        // 3) Enemies (include inactive)
        EnemySaveHandle[] enemies = FindObjectsOfType<EnemySaveHandle>(true);
        data.enemies = new EnemySaveData[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            var e = enemies[i];
            var eData = new EnemySaveData
            {
                id = e.enemyId,
                posX = e.transform.position.x,
                posY = e.transform.position.y,
                currentHP = e.CurrentHP,
                isDead = e.IsDead
            };
            data.enemies[i] = eData;
        }

        // 4) Animals (include inactive)
        AnimalSaveHandle[] animals = FindObjectsOfType<AnimalSaveHandle>(true);
        data.animals = new AnimalSaveData[animals.Length];
        for (int i = 0; i < animals.Length; i++)
        {
            var a = animals[i];
            var aData = new AnimalSaveData
            {
                id = a.animalId,
                posX = a.transform.position.x,
                posY = a.transform.position.y,
                flipX = a.GetFlipX(),
                hasHealth = a.HasHealth,
                currentHP = a.HasHealth ? a.CurrentHP : 0
            };
            data.animals[i] = aData;
        }

        return data;
    }

    // ============================
    // SAVE (cloud)
    // ============================
    public void Save()
    {
        FindPlayerRefs();
        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Không tìm thấy Player/PlayerStatus để save.");
            return;
        }

        SaveData data = BuildSaveDataForCurrentScene();

        string jsonCheck = JsonUtility.ToJson(data, true);
        Debug.Log("Dữ liệu gửi đi: " + jsonCheck);

        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        string playerName = PlayerIdentity.GetPlayerName();

        StartCoroutine(CloudSaveApi.SaveAll(playerId, playerName, data));
    }

    // ============================
    // LOAD (cloud)
    // ============================
    public void Load()
    {
        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        StartCoroutine(CloudSaveApi.LoadAll(playerId, OnCloudLoaded));
    }

    private void OnCloudLoaded(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[GameSaveManager] No cloud save found (or load failed).");
            return;
        }

        StartCoroutine(LoadSceneAndApply(data));
    }

    private IEnumerator LoadSceneAndApply(SaveData data)
    {
        // 1) Load đúng scene
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone)
            yield return null;

        // 2) Player
        FindPlayerRefs();
        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Không tìm thấy Player/PlayerStatus sau khi load scene.");
            yield break;
        }

        playerController.transform.position = new Vector3(
            data.player.posX,
            data.player.posY,
            playerController.transform.position.z
        );

        playerStatus.level = data.player.level;
        playerStatus.exp = data.player.exp;
        playerStatus.MaxHP = data.player.maxHP;
        playerStatus.CurrentHP = data.player.currentHP;

        // 3) Enemies
        EnemySaveHandle[] enemiesInScene = FindObjectsOfType<EnemySaveHandle>(true);
        foreach (var e in enemiesInScene)
        {
            var eData = data.enemies.FirstOrDefault(x => x.id == e.enemyId);
            if (eData == null) continue;

            if (eData.isDead || eData.currentHP <= 0)
            {
                e.gameObject.SetActive(false);
            }
            else
            {
                e.transform.position = new Vector3(
                    eData.posX,
                    eData.posY,
                    e.transform.position.z
                );
                e.CurrentHP = eData.currentHP;
                e.gameObject.SetActive(true);
            }
        }

        // 4) Animals
        AnimalSaveHandle[] animalsInScene = FindObjectsOfType<AnimalSaveHandle>(true);
        foreach (var a in animalsInScene)
        {
            var aData = data.animals.FirstOrDefault(x => x.id == a.animalId);
            if (aData == null) continue;

            a.gameObject.SetActive(true);
            a.ApplyState(
                aData.posX,
                aData.posY,
                aData.flipX,
                aData.hasHealth ? aData.currentHP : (int?)null
            );
        }

        Debug.Log("[GameSaveManager] Cloud load hoàn tất.");
    }

    // ============================
    // TEST PC
    // ============================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyDown(KeyCode.F9)) Load();
    }
}
