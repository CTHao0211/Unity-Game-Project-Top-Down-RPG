using TMPro;
using UnityEngine;

public class ConfirmSaveUI : MonoBehaviour
{
    public static ConfirmSaveUI Instance;

    [Header("UI")]
    public GameObject panel;          // panel con, mặc định tắt
    public TextMeshProUGUI messageText;

    private System.Action onConfirm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ẩn panel UI con
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(int slot, System.Action confirmAction)
    {
        onConfirm = confirmAction;

        if (messageText != null)
            messageText.text = $"Bạn có muốn ghi đè Save {slot} không?";

        if (panel != null)
            panel.SetActive(true); // bật panel con
    }

    public void OnClickYes()
    {
        if (panel != null) panel.SetActive(false);
        onConfirm?.Invoke();
        onConfirm = null;
    }

    public void OnClickNo()
    {
        if (panel != null) panel.SetActive(false);
        onConfirm = null;
    }
}
