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
        if (playerStatus == null)
            playerStatus = FindObjectOfType<PlayerStatus>();

        if (playerStatus != null)
        {
            playerStatus.onExpChanged += UpdateUI;
            playerStatus.onLevelUp += UpdateUI;
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (playerStatus != null)
        {
            playerStatus.onExpChanged -= UpdateUI;
            playerStatus.onLevelUp -= UpdateUI;
        }
    }

    public void UpdateUI()
    {
        if (playerStatus == null || expFill == null) return;

        float ratio = playerStatus.expToNextLevel > 0
            ? (float)playerStatus.exp / playerStatus.expToNextLevel
            : 0f;

        expFill.fillAmount = Mathf.Clamp01(ratio);

        if (expText != null)
            expText.text = $"{playerStatus.exp}/{playerStatus.expToNextLevel}";

        if (levelText != null)
            levelText.text = $"Level {playerStatus.level}";
    }

}

