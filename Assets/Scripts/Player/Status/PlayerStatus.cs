using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Level & EXP")]
    public int level = 1;
    public int exp = 0;

    [Header("Armor")]
    public int armorPhysical = 0;
    public int armorMagic = 0;

    [Header("Máu (PlayerHealth)")]
    public PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

#if UNITY_EDITOR
    // Chạy cả trong editor, mỗi lần m bấm vào object / đổi gì trong inspector
    private void OnValidate()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }
#endif

    public int CurrentHP
    {
        get => playerHealth != null ? playerHealth.currentHealth : 0;
        set
        {
            if (playerHealth == null) return;
            playerHealth.currentHealth = Mathf.Clamp(value, 0, playerHealth.maxHealth);
        }
    }

    public int MaxHP
    {
        get => playerHealth != null ? playerHealth.maxHealth : 0;
        set
        {
            if (playerHealth == null) return;
            playerHealth.maxHealth = value;
        }
    }
}
