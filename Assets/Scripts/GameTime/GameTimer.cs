using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    private float time;
    private bool isRunning = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!isRunning) return;
        time += Time.unscaledDeltaTime;
    }

    public float GetTime() => time;

    public void SetTime(float value)
    {
        time = Mathf.Max(0f, value);
    }

    public void ResetTimer()
    {
        time = 0f;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }
}
