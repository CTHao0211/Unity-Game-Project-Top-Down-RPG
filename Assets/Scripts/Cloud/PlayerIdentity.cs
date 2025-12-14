using UnityEngine;
using System;

public static class PlayerIdentity
{
    private const string KEY_ID = "player_id";
    private const string KEY_NAME = "player_name";

    public static string GetOrCreatePlayerId()
    {
        if (!PlayerPrefs.HasKey(KEY_ID) || string.IsNullOrEmpty(PlayerPrefs.GetString(KEY_ID)))
        {
            // UUID ngắn gọn, không dấu gạch
            string id = Guid.NewGuid().ToString("N");
            PlayerPrefs.SetString(KEY_ID, id);
            PlayerPrefs.Save();
            Debug.Log("[PlayerIdentity] Created playerId = " + id);
        }
        return PlayerPrefs.GetString(KEY_ID);
    }

    public static void SetPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = "Guest";

        name = name.Trim();
        PlayerPrefs.SetString(KEY_NAME, name);
        PlayerPrefs.Save();
        Debug.Log("[PlayerIdentity] Set playerName = " + name);
    }

    public static string GetPlayerName()
    {
        string n = PlayerPrefs.GetString(KEY_NAME, "Guest");
        return string.IsNullOrWhiteSpace(n) ? "Guest" : n;
    }

    // tiện debug / reset khi cần
    public static void ResetIdentity()
    {
        PlayerPrefs.DeleteKey(KEY_ID);
        PlayerPrefs.DeleteKey(KEY_NAME);
        PlayerPrefs.Save();
        Debug.Log("[PlayerIdentity] Identity reset.");
    }
}
