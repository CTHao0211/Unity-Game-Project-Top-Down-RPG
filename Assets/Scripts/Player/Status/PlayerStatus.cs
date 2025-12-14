using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Level & EXP")]
    public int level = 1;
    public int exp = 9;
    public int expToNextLevel = 10;

    [Header("Stats")]
    public int damage = 1;
    
    [Header("Health")]
    public PlayerHealth playerHealth;

    // Event cho UI
    public Action onExpChanged;
    public Action onLevelUp;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }
    public void ForceRefreshUI()
    {
        onExpChanged?.Invoke();
        onLevelUp?.Invoke();
    }


    public void AddExp(int amount)
    {
        exp += amount;
        CheckLevelUp();
        onExpChanged?.Invoke();
    }


    private void CheckLevelUp()
    {
        if (expToNextLevel <= 0) return;

        while (exp >= expToNextLevel)
        {
            exp -= expToNextLevel;
            LevelUp();
        }
    }


    private void LevelUp()
    {
        level++;
        expToNextLevel = Mathf.FloorToInt(expToNextLevel * 1.2f);

        if (playerHealth != null)
        {
            playerHealth.maxHealth += 5;
            playerHealth.currentHealth = playerHealth.maxHealth;
            playerHealth.UpdateHealthUI();
        }

        damage += 2;

        Debug.Log($"Level Up! Now Level {level}");
        onLevelUp?.Invoke();
    }

    public int CurrentHP
    {
        get => playerHealth != null ? playerHealth.currentHealth : 0;
        set
        {
            if (playerHealth != null)
                playerHealth.currentHealth = Mathf.Clamp(value, 0, playerHealth.maxHealth);
        }
    }

    public int MaxHP
    {
        get => playerHealth != null ? playerHealth.maxHealth : 0;
        set
        {
            if (playerHealth != null)
                playerHealth.maxHealth = value;
        }
    }

}
