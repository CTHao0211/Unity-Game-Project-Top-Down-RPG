using UnityEngine;

public class SavePanelUI : MonoBehaviour
{
    public static SavePanelUI Instance;

    public SaveSlotButton[] slots;
    public GameObject panel;

    private int selectedSlot = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (slots == null) return;

        foreach (var slot in slots)
        {
            if (slot != null)
                slot.Refresh();
        }
    }


    public void OnClickSlot(int slot)
    {
        selectedSlot = slot;

        ConfirmSaveUI.Instance.Show(
            slot,
            () =>
            {
                GameSaveManager.Instance.SaveToSlot(slot);
                RefreshSlots();
                panel.SetActive(true);
            }
        );
    }

    public PauseManager pauseManager; // gán trong Inspector

    public void OnClickBack()
    {
        // Tắt Save Panel
        panel.SetActive(false);

        // Tắt Confirm Panel nếu đang hiển thị
        if (ConfirmSaveUI.Instance != null)
            ConfirmSaveUI.Instance.panel.SetActive(false);

        // Bật lại Pause Menu
        if (pauseManager != null)
            pauseManager.pauseMenuUI.SetActive(true);
    }



    public void RefreshSlots()
    {
        foreach (var slot in slots)
            slot.Refresh();
    }
}
