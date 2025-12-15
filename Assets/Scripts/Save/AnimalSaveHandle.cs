﻿using UnityEngine;
using System;

public class AnimalSaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string animalId;

    [Header("Health (HealthBase)")]
    public HealthBase health;

    [Header("Respawn")]
    public float respawnDelay = 300f; // 5 phút

    private float deathTime = -1f;
    private Vector3 spawnPosition;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBase>();

        // 🔥 BẮT SỰ KIỆN CHẾT
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void Start()
    {
        // Lưu vị trí spawn gốc
        spawnPosition = transform.position;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(animalId))
        {
            animalId = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        if (health == null)
            health = GetComponent<HealthBase>();
    }

    // =====================
    // STATE
    // =====================

    public int CurrentHP => health != null ? health.currentHealth : 0;

    public bool IsDead =>
        !gameObject.activeSelf || (health != null && health.currentHealth <= 0);

    // =====================
    // DEATH
    // =====================

    private void HandleDeath()
    {
        // ⏱ ghi thời điểm chết (GAME TIME)
        deathTime = GameTimer.Instance.GetTime();
    }

    // =====================
    // SAVE
    // =====================

    public AnimalSaveData GetSaveData()
    {
        return new AnimalSaveData
        {
            id = animalId,
            posX = transform.position.x,
            posY = transform.position.y,
            currentHP = CurrentHP,
            isDead = IsDead,
            deathTime = deathTime,
            respawnDelay = respawnDelay
        };
    }

    // =====================
    // LOAD / BACKTRACK
    // =====================

    public void ApplyState(AnimalSaveData data)
    {
        float now = GameTimer.Instance.GetTime();

        // Nếu animal đã chết
        if (data.isDead)
        {
            // ⏱ đủ thời gian → respawn
            if (data.deathTime > 0 && now - data.deathTime >= data.respawnDelay)
            {
                Respawn();
                return;
            }

            // ❌ chưa đủ → giữ chết
            deathTime = data.deathTime;
            gameObject.SetActive(false);
            return;
        }

        // Animal còn sống
        transform.position = new Vector3(data.posX, data.posY, transform.position.z);

        if (health != null)
            health.ApplyLoadedHP(data.currentHP);

        gameObject.SetActive(true);
    }

    // =====================
    // RESPAWN
    // =====================

    private void Respawn()
    {
        deathTime = -1f;
        transform.position = spawnPosition;

        if (health != null)
            health.ApplyLoadedHP(health.maxHealth);

        gameObject.SetActive(true);
    }
}
