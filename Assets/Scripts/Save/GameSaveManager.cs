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
            Debug.Log("[AutoSave] Saved.");
        }
    }

    // ============================
    // SAVE (auto + nút)
    // ============================
    public void Save()
    {
        Debug.Log("[GameSaveManager] Save() được gọi");
        FindPlayerRefs();
        if (playerController == null || playerStatus == null)
        {
            Debug.LogError("[GameSaveManager] Không tìm thấy Player/PlayerStatus để save.");
            return;
        }

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
        data.player.armorPhysical = playerStatus.armorPhysical;
        data.player.armorMagic = playerStatus.armorMagic;

        // 3) Enemies
        EnemySaveHandle[] enemies = FindObjectsOfType<EnemySaveHandle>(true);
        data.enemies = new EnemySaveData[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            var e = enemies[i];
            var eData = new EnemySaveData();

            eData.id = e.enemyId;
            eData.posX = e.transform.position.x;
            eData.posY = e.transform.position.y;
            eData.currentHP = e.CurrentHP;
            eData.isDead = e.IsDead;

            data.enemies[i] = eData;
        }

        // 4) Animals
        AnimalSaveHandle[] animals = FindObjectsOfType<AnimalSaveHandle>(true);
        data.animals = new AnimalSaveData[animals.Length];
        for (int i = 0; i < animals.Length; i++)
        {
            var a = animals[i];
            var aData = new AnimalSaveData();

            aData.id = a.animalId;
            aData.posX = a.transform.position.x;
            aData.posY = a.transform.position.y;
            aData.flipX = a.GetFlipX();
            aData.currentHP = a.CurrentHP;
            aData.hasHealth = a.HasHealth;

            data.animals[i] = aData;
        }

        SaveSystem.SaveGame(data);
    }

    // ============================
    // LOAD
    // ============================
    public void Load()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("[GameSaveManager] Không có file save để load.");
            return;
        }

        StartCoroutine(LoadSceneAndApply(data));
    }

    private IEnumerator LoadSceneAndApply(SaveData data)
    {
        // 1) Load đúng scene
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

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
        playerStatus.armorPhysical = data.player.armorPhysical;
        playerStatus.armorMagic = data.player.armorMagic;

        // 3) Enemies
        EnemySaveHandle[] enemiesInScene = FindObjectsOfType<EnemySaveHandle>();
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
            }
        }

        // 4) Animals
        AnimalSaveHandle[] animalsInScene = FindObjectsOfType<AnimalSaveHandle>();
        foreach (var a in animalsInScene)
        {
            var aData = data.animals.FirstOrDefault(x => x.id == a.animalId);
            if (aData == null) continue;

            a.ApplyState(
                aData.posX,
                aData.posY,
                aData.flipX,
                aData.hasHealth ? aData.currentHP : (int?)null
            );
        }

        Debug.Log("[GameSaveManager] Load hoàn tất.");
    }

    // ============================
    // TEST PHÍM PC
    // ============================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyDown(KeyCode.F9)) Load();
    }
}
