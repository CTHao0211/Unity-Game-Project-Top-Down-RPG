using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotButton : MonoBehaviour
{
    public int slotIndex;

    [Header("UI")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI infoText;
    public Image background;

    private SaveData cachedData;

    public void Refresh()
    {
        cachedData = SaveSystem.LoadGame(slotIndex);

        if (cachedData == null)
        {
            slotTitle.text = $"Save {slotIndex}";
            infoText.text = "Empty Slot";
        }
        else
        {
            slotTitle.text = $"Save {slotIndex}";
            infoText.text =
                $"Player: {cachedData.playerName}\n" +
                $"Playtime: {FormatTime(cachedData.gameTime)}\n" +
                $"Saved: {cachedData.saveTime}";
        }
    }

public void SelectSlot()
{
    if (ConfirmSaveUI.Instance == null)
    {
        Debug.LogError("ConfirmSaveUI.Instance == null");
        return;
    }

    // Khi nhấn slot, ConfirmPanel sẽ bật
    ConfirmSaveUI.Instance.Show(slotIndex, () =>
    {
        GameSaveManager.Instance.SaveToSlot(slotIndex);
        SavePanelUI.Instance.RefreshSlots();
    });
}

    private string FormatTime(float time)
    {
        int m = Mathf.FloorToInt(time / 60);
        int s = Mathf.FloorToInt(time % 60);
        return $"{m:00}:{s:00}";
    }
}
