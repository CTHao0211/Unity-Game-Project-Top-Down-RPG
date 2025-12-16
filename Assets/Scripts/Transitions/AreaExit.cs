using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;       // Scene2
    [SerializeField] private string sceneTransitionName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

