using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public Image expFill;  
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    private void Start()
    {
        if (playerStatus != null)
        {
            playerStatus.onExpChanged += UpdateUI;
            playerStatus.onLevelUp += UpdateUI;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (playerStatus == null || expFill == null) return;

        // FillAmount = 0 -> 1
        expFill.fillAmount = (float)playerStatus.exp / playerStatus.expToNextLevel;

        if (expText != null)
            expText.text = $"{playerStatus.exp}/{playerStatus.expToNextLevel} to level up!";

        if (levelText != null)
            levelText.text = $"Level: {playerStatus.level}";
    }
}

