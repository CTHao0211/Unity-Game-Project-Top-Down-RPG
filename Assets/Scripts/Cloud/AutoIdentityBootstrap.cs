using UnityEngine;

public class AutoIdentityBootstrap : MonoBehaviour
{
    [Header("Chỉ tạo PlayerId. KHÔNG tự set PlayerName (vì tên phải register/login với server).")]
    [SerializeField] private bool logOnAwake = true;

    private void Awake()
    {
        string id = PlayerIdentity.GetOrCreatePlayerId();

        if (logOnAwake)
        {
            string name = PlayerIdentity.GetPlayerName();
            Debug.Log($"[AutoIdentityBootstrap] playerId={id}, playerName={name}");
        }
    }
}
