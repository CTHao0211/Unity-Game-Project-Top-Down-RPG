using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class LeaderboardApi
{
    // Server bạn đang dùng (index.js)
    public static string BASE_URL = "https://cloud-save-server.onrender.com";

    [Serializable]
    private class SubmitPayload
    {
        public string playerId;
        public string playerName;
        public int completionTimeMs;
    }

    // Server trả: { status: "success", data: [...] }
    [Serializable]
    private class LeaderboardResponse
    {
        public string status;
        public LeaderboardRow[] data;
        public string error;
    }

    // Lưu ý: phải đúng snake_case y như server/supabase trả về
    [Serializable]
    private class LeaderboardRow
    {
        public string player_id;
        public string player_name;
        public int completion_time_ms;
        public string completed_at;
    }

    public static IEnumerator SubmitRun(string playerId, string playerName, int completionTimeMs)
    {
        var payload = new SubmitPayload
        {
            playerId = (playerId ?? "").Trim(),
            playerName = (playerName ?? "Player").Trim(),
            completionTimeMs = Mathf.Max(0, completionTimeMs)
        };

        var json = JsonUtility.ToJson(payload);

        var req = new UnityWebRequest(BASE_URL + "/api/submit-run", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[LeaderboardApi] Submit error: " + req.error);
            Debug.LogError("[LeaderboardApi] Response: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log("[LeaderboardApi] Submit OK: " + req.downloadHandler.text);
        }
    }

    // Trả về SaveData[] để LeaderboardUIManager dùng lại (playerName + completionTime)
    public static IEnumerator LoadLeaderboard(int limit, Action<SaveData[]> onLoaded)
    {
        limit = Mathf.Clamp(limit, 1, 200);
        var url = BASE_URL + "/api/leaderboard?limit=" + limit;

        var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[LeaderboardApi] Load error: " + req.error);
            Debug.LogError("[LeaderboardApi] Response: " + req.downloadHandler.text);
            onLoaded?.Invoke(null);
            yield break;
        }

        var raw = req.downloadHandler.text;
        var resp = JsonUtility.FromJson<LeaderboardResponse>(raw);
        if (resp == null || resp.data == null)
        {
            Debug.LogError("[LeaderboardApi] Parse failed: resp/data is null (JsonUtility không parse được format?)");
            onLoaded?.Invoke(new SaveData[0]);
            yield break;
        }

        var mapped = resp.data
            .Where(r => r != null && r.completion_time_ms > 0)
            .Select(r => new SaveData
            {
                playerName = string.IsNullOrEmpty(r.player_name) ? "Player" : r.player_name,
                completionTime = r.completion_time_ms / 1000f
            })
            .ToArray();

        onLoaded?.Invoke(mapped);
    }
}
