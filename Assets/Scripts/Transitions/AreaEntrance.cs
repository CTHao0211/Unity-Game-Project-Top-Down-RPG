using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;

    private void Start()
    {
        // Không có SceneManagement thì thôi, khỏi làm gì
        if (SceneManagement.Instance == null)
        {
            Debug.LogWarning("AreaEntrance: SceneManagement.Instance is null");
            return;
        }

        if (transitionName == SceneManagement.Instance.SceneTransitionName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
            }
        }
    }
}
