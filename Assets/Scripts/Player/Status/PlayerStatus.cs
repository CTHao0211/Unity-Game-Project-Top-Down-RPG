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

    public void AddExp(int amount)
    {
        exp += amount;
        onExpChanged?.Invoke();
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
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

        // Tăng stats
        if (playerHealth != null)
        {
            playerHealth.maxHealth += 5;                       // tăng max HP
            playerHealth.currentHealth = playerHealth.maxHealth; // hồi full HP
            playerHealth.UpdateHealthUI();                     // cập nhật thanh HP ngay lập tức
        }


        damage += 2;


        Debug.Log($"Level Up! Now Level {level}");

        onLevelUp?.Invoke();
        onExpChanged?.Invoke();
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
