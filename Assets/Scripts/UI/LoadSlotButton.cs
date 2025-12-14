using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSlotButton : MonoBehaviour
{
    public int slotIndex; // Slot này đại diện cho save slot nào

    [Header("UI References")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI infoText;
    public Button button;

    private void Start()
    {
        Refresh();
        button.onClick.AddListener(OnClickLoadSlot);
    }

    // Cập nhật thông tin slot
    public void Refresh()
    {
        var data = SaveSystem.LoadGame(slotIndex);

        if (data == null)
        {
            slotTitle.text = $"Load {slotIndex}";
            infoText.text = "Empty Slot";
            button.interactable = false; // không thể load slot trống
        }
        else
        {
            slotTitle.text = $"Save {slotIndex}";
            infoText.text =
                $"Player: {data.playerName}\n" +
                $"Playtime: {FormatTime(data.gameTime)}\n" +
                $"Saved: {data.saveTime}";
            button.interactable = true;
        }
    }

    // Khi nhấn nút load slot
    public void OnClickLoadSlot()
    {
        Debug.Log($"Load slot {slotIndex}");
        GameSaveManager.Instance.LoadFromSlot(slotIndex);
    }

    // Format thời gian MM:SS
    private string FormatTime(float time)
    {
        int m = Mathf.FloorToInt(time / 60);
        int s = Mathf.FloorToInt(time % 60);
        return $"{m:00}:{s:00}";
    }
}
