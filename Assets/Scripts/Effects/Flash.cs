using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Flash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.12f;

    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private Coroutine runningFlash;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        mpb = new MaterialPropertyBlock();

        // dùng color thực tế của SpriteRenderer làm màu gốc
        originalColor = sr.color;
    }

    /// <summary>
    /// Hàm public để EnemyHealth gọi flash
    /// </summary>
    public void StartFlash()
    {
        if (runningFlash != null)
            StopCoroutine(runningFlash);

        runningFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // bật flash
        sr.GetPropertyBlock(mpb);
        mpb.SetColor(ColorProp, flashColor);
        sr.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(flashDuration);

        // trả về màu gốc
        sr.GetPropertyBlock(mpb);
        mpb.SetColor(ColorProp, originalColor);
        sr.SetPropertyBlock(mpb);

        runningFlash = null;
    }
}
