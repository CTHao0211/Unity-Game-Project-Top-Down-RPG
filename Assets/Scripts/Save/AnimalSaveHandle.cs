﻿using UnityEngine;
using System;

public class AnimalSaveHandle : MonoBehaviour
{
    [Header("ID tự sinh (đừng sửa tay)")]
    public string animalId;

    [Header("Health (HealthBase) - nếu có")]
    public HealthBase health;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBase>();
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

    public int CurrentHP
    {
        get => health != null ? health.currentHealth : 0;
        set
        {
            if (health == null) return;
            health.currentHealth = Mathf.Clamp(value, 0, health.maxHealth);
        }
    }

    public bool IsDead
    {
        get
        {
            if (!gameObject.activeSelf) return true;
            if (health == null) return false;
            return health.currentHealth <= 0;
        }
    }


    public Vector3 GetPosition() => transform.position;

    /// <summary>
    /// Áp dụng state khi load game
    /// </summary>
    public void ApplyState(float x, float y, int hp, bool isDead)
    {
        if (isDead || hp <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = new Vector3(x, y, transform.position.z);

        if (health != null)
            health.ApplyLoadedHP(hp); 
    }
}