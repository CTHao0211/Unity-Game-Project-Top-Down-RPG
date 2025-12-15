using UnityEngine;
using System;

public class EnemySaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string enemyId;

    [Header("Health")]
    public HealthBase health;

    [Header("Respawn Settings")]
    public float respawnDelay = 300f; // giây

    private float deathTime = -1f;
    private Vector3 spawnPosition;

    // =========================
    // LIFECYCLE
    // =========================
    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBase>();

        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void Start()
    {
        spawnPosition = transform.position;
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            enemyId = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        if (health == null)
            health = GetComponent<HealthBase>();
    }

    // =========================
    // STATE
    // =========================
    public bool IsDead =>
        !gameObject.activeSelf ||
        (health != null && health.currentHealth <= 0);

    public int CurrentHP =>
        health != null ? health.currentHealth : 0;

    // =========================
    // EVENTS
    // =========================
    private void HandleDeath()
    {
        // Ghi lại thời điểm chết (dùng GameTimer toàn cục)
        deathTime = GameTimer.Instance != null
            ? GameTimer.Instance.GetTime()
            : Time.time;
    }

    // =========================
    // SAVE
    // =========================
    public EnemySaveData GetSaveData()
    {
        return new EnemySaveData
        {
            id = enemyId,
            posX = transform.position.x,
            posY = transform.position.y,
            currentHP = CurrentHP,
            isDead = IsDead,
            deathTime = deathTime,
            respawnDelay = respawnDelay
        };
    }

    // =========================
    // LOAD / BACKTRACK
    // =========================
    public void ApplyState(EnemySaveData data)
    {
        float now = GameTimer.Instance != null
            ? GameTimer.Instance.GetTime()
            : Time.time;

        // Enemy đã chết
        if (data.isDead)
        {
            // ⏱ Đủ thời gian → respawn
            if (data.deathTime > 0 &&
                now - data.deathTime >= data.respawnDelay)
            {
                Respawn();
                return;
            }

            // ❌ Chưa đủ → giữ chết
            deathTime = data.deathTime;
            gameObject.SetActive(false);
            return;
        }

        // Enemy còn sống
        transform.position = new Vector3(
            data.posX,
            data.posY,
            transform.position.z
        );

        if (health != null)
            health.ApplyLoadedHP(data.currentHP);

        gameObject.SetActive(true);
    }

    // =========================
    // RESPAWN
    // =========================
    private void Respawn()
    {
        deathTime = -1f;
        transform.position = spawnPosition;

        if (health != null)
            health.ApplyLoadedHP(health.maxHealth);

        gameObject.SetActive(true);
    }
}
