using UnityEngine;

public class DebugWinTrigger : MonoBehaviour
{
    public bool enableDebugWin = true;
    public KeyCode winKey = KeyCode.F10;

    void Update()
    {
        if (!enableDebugWin) return;
        if (Input.GetKeyDown(winKey)) TriggerWin();
    }

    public void TriggerWin()
    {
        var gsm = GameSaveManager.Instance;
        if (gsm == null) return;

        float timeSec = (gsm.gameTimer != null) ? gsm.gameTimer.GetTime() : 0f;
        int timeMs = Mathf.FloorToInt(timeSec * 1000f);

        string playerId = PlayerIdentity.GetOrCreatePlayerId();
        string playerName = PlayerIdentity.GetPlayerName();

        gsm.StartCoroutine(LeaderboardApi.SubmitRun(playerId, playerName, timeMs));
    }
}
