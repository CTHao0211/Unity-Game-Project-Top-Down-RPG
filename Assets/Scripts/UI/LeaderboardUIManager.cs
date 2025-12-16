using UnityEngine;
using TMPro;
using System.Linq;

public class LeaderboardUIManager : MonoBehaviour
{
    [SerializeField] private Transform[] entries;

    public void ShowLeaderboardUI()
    {
        if (entries == null || entries.Length == 0)
        {
            Debug.LogError("[LeaderboardUI] entries chưa gán trong Inspector.");
            return;
        }

        var gsm = GameSaveManager.Instance;
        if (gsm == null)
        {
            Debug.LogError("[LeaderboardUI] GameSaveManager.Instance NULL.");
            return;
        }

        var data = gsm.GetLeaderboardData();
        if (data == null || data.Length == 0)
        {
            Debug.LogWarning("[LeaderboardUI] Không có dữ liệu leaderboard từ server.");
            // tắt hết entry
            for (int i = 0; i < entries.Length; i++)
                if (entries[i] != null) entries[i].gameObject.SetActive(false);
            return;
        }

        Debug.Log($"[LeaderboardUI] Có {data.Length} người chơi trong danh sách. entriesUI={entries.Length}");

        var top = data
            .Where(d => d != null && d.completionTime > 0)
            .GroupBy(d => d.playerName)                     // gom theo tên
            .Select(g => g.OrderBy(d => d.completionTime).First()) // chọn bản tốt nhất
            .OrderBy(d => d.completionTime)                // sắp xếp theo thời gian tốt nhất
            .Take(entries.Length)
            .ToList();


        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            if (entry == null)
            {
                Debug.LogError($"[LeaderboardUI] entries[{i}] bị null (chưa gán).");
                continue;
            }

            bool hasData = i < top.Count;
            entry.gameObject.SetActive(hasData);

            if (!hasData) continue;

            // Tìm đúng theo tên object con (khuyến nghị bạn đặt tên như này)
            var nameText = entry.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
            var timeText = entry.Find("TxtTime")?.GetComponent<TextMeshProUGUI>();

            // Fallback nếu bạn chưa đặt tên TxtName/TxtTime:
            if (nameText == null || timeText == null)
            {
                var tmps = entry.GetComponentsInChildren<TextMeshProUGUI>(true);
                Debug.Log($"[LeaderboardUI] entry[{i}] TMP count={tmps.Length} ({string.Join(", ", tmps.Select(t => t.name))})");

                if (tmps.Length >= 2)
                {
                    nameText = tmps[0];
                    timeText = tmps[1];
                }
            }

            if (nameText == null || timeText == null)
            {
                Debug.LogError($"[LeaderboardUI] entry[{i}] không tìm thấy 2 TextMeshProUGUI để set text.");
                continue;
            }

            nameText.text = top[i].playerName;
            timeText.text = $"{top[i].completionTime:0.0}s";

        }
    }
}
