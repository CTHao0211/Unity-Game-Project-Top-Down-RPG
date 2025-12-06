using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;   // TMP component
    public float floatSpeed = 1f;
    public float fadeSpeed = 1f;

    private Color currentColor; // lưu màu đang dùng, bao gồm alpha

    private void Awake()
    {
    if (textMesh == null)
        textMesh = GetComponentInChildren<TextMeshProUGUI>();


        // Lấy màu gốc từ prefab
        currentColor = textMesh.color;
    }

    /// <summary>
    /// Setup damage popup
    /// </summary>
    /// <param name="dmg">Số damage hiển thị</param>
    /// <param name="overrideColor">Màu tuỳ chọn, nếu null sẽ dùng màu prefab</param>
    public void Setup(int dmg, Color? overrideColor = null)
    {
         textMesh.text = dmg.ToString();

        // Nếu có màu override, dùng nó; nếu không, dùng màu prefab
        currentColor = overrideColor ?? textMesh.color;

        // Reset alpha
        currentColor.a = 1f;
        textMesh.color = currentColor;
    }

    private void Update()
    {
        // Popup nổi lên
        transform.localPosition += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade alpha, giữ nguyên RGB
        currentColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = currentColor;

        if (currentColor.a <= 0f)
            Destroy(gameObject);
    }
}
