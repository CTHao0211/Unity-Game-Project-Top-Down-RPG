using UnityEngine;

public class AutoIdentityBootstrap : MonoBehaviour
{
    [Header("Nếu chưa có tên, tự gán tạm")]
    public string defaultName = "Guest";

    private void Awake()
    {
        // tạo playerId nếu chưa có
        var id = PlayerIdentity.GetOrCreatePlayerId();

        // nếu chưa có name, gán tạm
        var name = PlayerIdentity.GetPlayerName();
        if (string.IsNullOrEmpty(name) || name == "Guest")
        {
            PlayerIdentity.SetPlayerName(defaultName);
        }

        Debug.Log($"[AutoIdentityBootstrap] playerId={id}, playerName={PlayerIdentity.GetPlayerName()}");
    }
}
