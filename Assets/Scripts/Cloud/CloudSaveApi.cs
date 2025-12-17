using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class CloudSaveApi
{
    public static string BASE_URL = "https://cloud-save-server.onrender.com";

    [Serializable]
    private class SaveWrapper
    {
        public string playerId;
        public string playerName;
        public SaveData saveData;
    }

    [Serializable]
    private class LoadResponseWrapper
    {
        public string status;   
        public SaveData saveData;
    }

    public static IEnumerator SaveAll(string playerId, string playerName, SaveData data)
    {
        var payload = new SaveWrapper
        {
            playerId = playerId,
            playerName = playerName,
            saveData = data
        };

        string json = JsonUtility.ToJson(payload);

        var req = new UnityWebRequest(BASE_URL + "/save-all", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[CloudSaveApi] Save error: " + req.error);
            Debug.LogError("[CloudSaveApi] Response: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log("[CloudSaveApi] Save OK: " + req.downloadHandler.text);
        }
    }

    public static IEnumerator LoadAll(string playerId, Action<SaveData[]> onLoaded)
    {
        var req = UnityWebRequest.Get(BASE_URL + "/load-all?playerId=" + playerId);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[CloudSaveApi] Load error: " + req.error);
            onLoaded?.Invoke(null);
            yield break;
        }

        string raw = req.downloadHandler.text;
        Debug.Log("[CloudSaveApi] Load raw: " + raw);

        var wrapperArray = JsonUtility.FromJson<SaveDataArrayWrapper>(raw); 
        // cần định nghĩa SaveDataArrayWrapper để JsonUtility parse thành mảng
        onLoaded?.Invoke(wrapperArray.saveDataArray);
    }

    [Serializable]
    private class SaveDataArrayWrapper
    {
        public SaveData[] saveDataArray;
    }

}
