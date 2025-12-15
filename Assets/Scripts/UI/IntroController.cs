using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textUI;
    public CanvasGroup introGroup;
    public GameObject menuGroup;

    [Header("Voice")]
    public AudioSource voiceSource;
    public AudioClip[] voiceClips;

    [Header("Settings")]
    public float typingSpeed = 0.04f;
    public float fadeSpeed = 1f;
    public string nextSceneName = "Scene1";

    [TextArea(3, 6)]
    public string[] paragraphs;

    int index = 0;
    bool isTyping = false;

    const string INTRO_KEY = "INTRO_SHOWN";

    void Start()
    {
        // Mặc định: intro KHÔNG chạy
        introGroup.alpha = 0;
        introGroup.gameObject.SetActive(false);

        // Menu luôn bật
        menuGroup.SetActive(true);
    }

    // Gọi từ nút "Chơi Mới"
    public void StartIntro()
    {
        PlayerPrefs.DeleteKey(INTRO_KEY);

        menuGroup.SetActive(false);
        introGroup.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
        StartCoroutine(TypeParagraph());
    }

    void Update()
    {
        if (!introGroup.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetMouseButtonDown(0)
            || Input.touchCount > 0)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            textUI.text = paragraphs[index];
            isTyping = false;

            if (voiceSource.isPlaying)
                voiceSource.Stop();
        }
        else
        {
            index++;
            if (index < paragraphs.Length)
            {
                StartCoroutine(TypeParagraph());
            }
            else
            {
                PlayerPrefs.SetInt(INTRO_KEY, 1);
                PlayerPrefs.Save();
                StartCoroutine(FadeOutAndLoad());
            }
        }
    }

    IEnumerator TypeParagraph()
    {
        isTyping = true;
        textUI.text = "";

        // Play voice
        if (voiceClips != null && index < voiceClips.Length)
        {
            voiceSource.clip = voiceClips[index];
            voiceSource.Play();
        }

        foreach (char c in paragraphs[index])
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator FadeIn()
    {
        introGroup.alpha = 0;
        while (introGroup.alpha < 1)
        {
            introGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutAndLoad()
    {
        while (introGroup.alpha > 0)
        {
            introGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
