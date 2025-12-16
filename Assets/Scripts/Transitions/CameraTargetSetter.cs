using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraTargetSetter : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Collider2D boundingShape2D; // Gán collider map ở Inspector

    private void OnEnable()
    {
        // Lấy VCam nếu chưa gán
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // Lấy player từ Singleton (player DontDestroyOnLoad)
        if (PlayerControllerCombined.Instance != null)
        {
            Transform player = PlayerControllerCombined.Instance.transform;

            // Follow player, không LookAt
            virtualCamera.Follow = player;
            virtualCamera.LookAt = null;

            // Reset rotation
            virtualCamera.transform.rotation = Quaternion.identity;

            // Setup confiner nếu có bounding
            SetupConfiner();

            // Tắt xoay camera theo target
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                transposer.m_TrackedObjectOffset = Vector3.zero;
            }
        }
    }

    private void SetupConfiner()
    {
        if (boundingShape2D == null) return;

        var confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        if (confiner == null)
            confiner = virtualCamera.gameObject.AddComponent<CinemachineConfiner>();

        // Chỉ gán bounding shape, không dùng các field private cũ
        confiner.m_BoundingShape2D = boundingShape2D;
        confiner.InvalidatePathCache();
    }
}
