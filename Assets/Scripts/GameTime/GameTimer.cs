using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;   // kéo TimerText vào đây

    private float elapsedTime = 0f;
    private bool isRunning = true;

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // Nếu không muốn chữ "Time:" thì bỏ đi
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    // Sau này nếu cần dùng:
    public void SetTime(float time)
    {
        elapsedTime = time;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    public float GetTime()
    {
        return elapsedTime;
    }
}
