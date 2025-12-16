using UnityEngine;

public class GlobalLightSingleton : MonoBehaviour
{
    private static GlobalLightSingleton instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);   // ❗ xoá cái thừa
            return;
        }

        instance = this;

        // ❗ BẮT BUỘC: đưa lên root
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }
}
