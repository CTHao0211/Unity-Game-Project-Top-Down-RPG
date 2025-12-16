﻿using UnityEngine;
using System;

public class AnimalSaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string animalId;

    [Header("Health (HealthBase)")]
    public HealthBase health;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBase>();

        if (health != null)
            health.OnDeath += HandleDeath;
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

    public int CurrentHP => health != null ? health.currentHealth : 0;
    public bool IsDead => !gameObject.activeSelf || (health != null && health.currentHealth <= 0);

    private void HandleDeath() { }

    public AnimalSaveData GetSaveData()
    {
        return new AnimalSaveData
        {
            id = animalId,
            posX = transform.position.x,
            posY = transform.position.y,
            currentHP = CurrentHP,
            isDead = IsDead
        };
    }

    public void ApplyState(AnimalSaveData data)
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
