using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class AuthApi
{
    public static string BASE_URL = "https://cloud-save-server.onrender.com";

    [Serializable]
    public class Payload
    {
        public string playerId;
        public string playerName;
    }

    [Serializable]
    public class Resp
    {
        public bool ok;
        public string error;
        public string name;
        public string player_id;
    }

    [Serializable]
    public class CheckResp
    {
        public bool ok;
        public bool available;
        public string error;
    }

    private static Resp TryParseResp(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        try { return JsonUtility.FromJson<Resp>(raw); }
        catch { return null; }
    }

    private static CheckResp TryParseCheck(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        try { return JsonUtility.FromJson<CheckResp>(raw); }
        catch { return null; }
    }

    public static IEnumerator Register(string playerId, string playerName, Action<Resp> onDone)
    {
        var p = new Payload { playerId = playerId, playerName = playerName };
        var json = JsonUtility.ToJson(p);

        using (var req = new UnityWebRequest(BASE_URL + "/api/register", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            var raw = req.downloadHandler != null ? req.downloadHandler.text : "";
            var resp = TryParseResp(raw);

            if (resp != null)
            {
                onDone?.Invoke(resp);
                yield break;
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[AuthApi] Register request failed: " + req.error + " raw=" + raw);
                onDone?.Invoke(new Resp { ok = false, error = "network_error" });
                yield break;
            }

            onDone?.Invoke(new Resp { ok = false, error = "parse_error" });
        }
    }

    public static IEnumerator Login(string playerId, string playerName, Action<Resp> onDone)
    {
        var p = new Payload { playerId = playerId, playerName = playerName };
        var json = JsonUtility.ToJson(p);

        using (var req = new UnityWebRequest(BASE_URL + "/api/login", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            var raw = req.downloadHandler != null ? req.downloadHandler.text : "";
            var resp = TryParseResp(raw);

            if (resp != null)
            {
                onDone?.Invoke(resp);
                yield break;
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[AuthApi] Login request failed: " + req.error + " raw=" + raw);
                onDone?.Invoke(new Resp { ok = false, error = "network_error" });
                yield break;
            }

            onDone?.Invoke(new Resp { ok = false, error = "parse_error" });
        }
    }

    public static IEnumerator CheckNameAvailable(string playerName, Action<CheckResp> onDone)
    {
        string name = Uri.EscapeDataString((playerName ?? "").Trim());

        using (var req = UnityWebRequest.Get(BASE_URL + "/api/name-available?name=" + name))
        {
            yield return req.SendWebRequest();

            var raw = req.downloadHandler != null ? req.downloadHandler.text : "";
            var resp = TryParseCheck(raw);

            if (resp != null)
            {
                onDone?.Invoke(resp);
                yield break;
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[AuthApi] CheckName request failed: " + req.error + " raw=" + raw);
                onDone?.Invoke(new CheckResp { ok = false, available = false, error = "network_error" });
                yield break;
            }

            onDone?.Invoke(new CheckResp { ok = false, available = false, error = "parse_error" });
        }
    }
}
