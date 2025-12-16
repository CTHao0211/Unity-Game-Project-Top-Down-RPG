using UnityEngine;
using System;

public class EnemySaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string enemyId;

    [Header("Health")]
    public HealthBase health;

    private void Awake()
    {
        if (string.IsNullOrEmpty(enemyId) || EnemyIdExistsInScene(enemyId))
        {
            enemyId = Guid.NewGuid().ToString();
            Debug.Log($"[EnemySaveHandle] Generated new enemyId: {enemyId}");
        }
        if (health == null)
            health = GetComponent<HealthBase>();

        if (health != null)
            health.OnDeath += HandleDeath;
    }
    private bool EnemyIdExistsInScene(string id)
    {
        foreach (var e in FindObjectsOfType<EnemySaveHandle>())
        {
            if (e != this && e.enemyId == id)
                return true;
        }
        return false;
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
    public bool IsDead => !gameObject.activeSelf || (health != null && health.currentHealth <= 0);
    public int CurrentHP => health != null ? health.currentHealth : 0;

    // =========================
    // EVENTS
    // =========================
    private void HandleDeath() { }

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
            isDead = IsDead
        };
    }

    // =========================
    // LOAD / BACKTRACK
    // =========================
    public void ApplyState(EnemySaveData data)
    {
        if (data.isDead)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = new Vector3(data.posX, data.posY, transform.position.z);

        if (health != null)
            health.ApplyLoadedHP(data.currentHP);

        gameObject.SetActive(true);
    }
}
