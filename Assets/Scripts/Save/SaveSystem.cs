using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string fileName = "save.json";

    private static string GetFullPath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetFullPath();

        File.WriteAllText(path, json);

        Debug.Log("[SaveSystem] Saved to: " + path);
        Debug.Log("Persistent Path = " + Application.persistentDataPath);
    }

    public static SaveData LoadGame()
    {
        string path = GetFullPath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[SaveSystem] Save file not found: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("[SaveSystem] Loaded from: " + path);
        return data;
    }

    public static bool HasSave()
    {
        return File.Exists(GetFullPath());
    }
}
