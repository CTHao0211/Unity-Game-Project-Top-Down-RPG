using UnityEngine;

public class LoadPanelUI : MonoBehaviour
{
    public static LoadPanelUI Instance;

    [Header("Slot Buttons")]
    public LoadSlotButton[] slotButtons; // Kéo thả các slot button vào đây

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshSlots();
    }

    // Cập nhật tất cả slot
    public void RefreshSlots()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            slotButtons[i].Refresh();
        }
    }

    // Hiển thị panel
    public void Show()
    {
        gameObject.SetActive(true);
        RefreshSlots();
    }

    // Ẩn panel
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
