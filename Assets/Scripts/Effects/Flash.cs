using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour
{
    private Material mat;
    public float flashDuration = 0.1f;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    public void FlashWhite()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        mat.SetFloat("_FlashAmount", 1f);
        yield return new WaitForSeconds(flashDuration);
        mat.SetFloat("_FlashAmount", 0f);
    }
}
