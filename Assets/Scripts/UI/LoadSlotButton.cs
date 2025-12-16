using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadSlotButton : MonoBehaviour
{
    public int slotIndex;

    [Header("UI References")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI infoText;
    public Button button;

    private void Start()
    {
        Refresh();
        button.onClick.AddListener(OnClickLoadSlot);
    }

    public void Refresh()
    {
        var data = SaveSystem.LoadGame(slotIndex);

        if (data == null)
        {
            slotTitle.text = $"Load {slotIndex}";
            infoText.text = "Empty Slot";
            button.interactable = false;
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

    public void OnClickLoadSlot()
    {
        Debug.Log($"Load slot {slotIndex}");

        // Load dữ liệu save slot
        GameSaveManager.Instance.LoadFromSlot(slotIndex);

        // Load Scene game chính
        SceneManager.LoadScene("Scene1");
    }

    private string FormatTime(float time)
    {
        int m = Mathf.FloorToInt(time / 60);
        int s = Mathf.FloorToInt(time % 60);
        return $"{m:00}:{s:00}";
    }

}
