using UnityEngine;

public class GlobalLightSingleton : MonoBehaviour
{
    private static GlobalLightSingleton instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
