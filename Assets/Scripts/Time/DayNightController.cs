using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNight2DController : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Thời gian 1 ngày (tính bằng phút, trong game).")]
    public float dayLengthInMinutes = 2f;

    [Range(0f, 1f)]
    [Tooltip("Thời điểm trong ngày (0 = nửa đêm, 0.25 = sáng, 0.5 = trưa, 0.75 = chiều tối, 1 = nửa đêm tiếp).")]
    public float timeOfDay = 0f;

    [Header("2D Global Light")]
    [Tooltip("Light2D chế độ Global trong scene.")]
    public Light2D globalLight;

    [Tooltip("Gradient màu ánh sáng theo thời gian trong ngày.")]
    public Gradient lightColorGradient;

    [Tooltip("Đồ thị cường độ ánh sáng theo thời gian trong ngày.")]
    public AnimationCurve lightIntensityCurve;

    void Reset()
    {
        // Tự tìm Global Light2D nếu chưa gán
        if (globalLight == null)
        {
            Light2D[] lights = FindObjectsOfType<Light2D>();
            foreach (var l in lights)
            {
                if (l.lightType == Light2D.LightType.Global)
                {
                    globalLight = l;
                    break;
                }
            }
        }

        // Gradient default: đêm → sáng → trưa → chiều → tối
        if (lightColorGradient == null)
            lightColorGradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[5];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        colorKeys[0] = new GradientColorKey(new Color(0.02f, 0.05f, 0.1f), 0.0f);   // đêm
        colorKeys[1] = new GradientColorKey(new Color(1.0f, 0.6f, 0.3f), 0.25f);   // bình minh
        colorKeys[2] = new GradientColorKey(new Color(1.0f, 0.95f, 0.8f), 0.5f);   // trưa
        colorKeys[3] = new GradientColorKey(new Color(1.0f, 0.5f, 0.3f), 0.75f);   // hoàng hôn
        colorKeys[4] = new GradientColorKey(new Color(0.02f, 0.05f, 0.1f), 1.0f);  // đêm

        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        lightColorGradient.SetKeys(colorKeys, alphaKeys);

        // Curve default: đêm tắt, sáng dần, trưa mạnh, chiều giảm, tối tắt
        lightIntensityCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f),
            new Keyframe(0.2f, 0.3f),
            new Keyframe(0.5f, 1.0f),
            new Keyframe(0.8f, 0.3f),
            new Keyframe(1.0f, 0.0f)
        );
    }

    void Update()
    {
        if (globalLight == null || dayLengthInMinutes <= 0f)
            return;

        float dayLengthInSeconds = dayLengthInMinutes * 60f;
        timeOfDay += Time.deltaTime / dayLengthInSeconds;
        if (timeOfDay > 1f)
            timeOfDay -= 1f;

        UpdateLighting(timeOfDay);
    }

    void UpdateLighting(float t)
    {
        if (lightColorGradient != null)
            globalLight.color = lightColorGradient.Evaluate(t);

        if (lightIntensityCurve != null)
            globalLight.intensity = lightIntensityCurve.Evaluate(t);
    }
}
