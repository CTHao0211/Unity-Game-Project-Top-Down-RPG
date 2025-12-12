using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Flash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.12f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine runningFlash;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        originalColor = sr.color;
    }

    public void StartFlash()
    {
        if (runningFlash != null)
            StopCoroutine(runningFlash);

        runningFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // đổi sang màu trắng (flash)
        sr.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        // trả về màu gốc
        sr.color = originalColor;

        runningFlash = null;
    }
}
