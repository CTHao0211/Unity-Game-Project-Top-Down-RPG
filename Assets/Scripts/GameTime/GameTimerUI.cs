using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private void Update()
    {
        if (GameTimer.Instance == null || timerText == null)
            return;

        float t = GameTimer.Instance.GetTime();
        int totalSeconds = Mathf.FloorToInt(t);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

    }
}
