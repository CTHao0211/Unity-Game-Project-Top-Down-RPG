using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class LeaderboardApi
{
    public static string BASE_URL = "https://cloud-save-server.onrender.com";

    [Serializable]
    private class SubmitPayload
    {
        public string playerId;
        public string playerName;
        public int completionTimeMs;
    }

    public static IEnumerator SubmitRun(string playerId, string playerName, int completionTimeMs)
    {
        var payload = new SubmitPayload
        {
            playerId = playerId,
            playerName = playerName,
            completionTimeMs = completionTimeMs
        };

        var json = JsonUtility.ToJson(payload);

        var req = new UnityWebRequest(BASE_URL + "/api/submit-run", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
    }
}
