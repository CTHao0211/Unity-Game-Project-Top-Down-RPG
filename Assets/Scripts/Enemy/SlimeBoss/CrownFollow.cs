using UnityEngine;
using System.Collections;
public class CrownFollow : MonoBehaviour
{
    public Transform body;      // slime body
    public float baseHeight = 0.4f;
    public float followStrength = 0.1f;
    public SpriteRenderer crownSprite;

    [Header("Shake Settings")]
    public float shakeStrength = 0.05f;
    public float shakeDuration = 0.2f;

    [Header("Color Settings")]
    public Color attackColor = Color.red;

    Vector3 originalLocalPos;
    Color originalColor;
    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalColor = crownSprite.color;
    }
    public void OnBossAttack()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeAndColor());
    }
    

    IEnumerator ShakeAndColor()
    {
    float timer = 0f;

    crownSprite.color = attackColor;

    while (timer < shakeDuration)
    {
        float offsetX = Random.Range(-shakeStrength, shakeStrength);
        float offsetY = Random.Range(-shakeStrength, shakeStrength);

        transform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0);

        timer += Time.deltaTime;
        yield return null;
    }

    transform.localPosition = originalLocalPos;
    crownSprite.color = originalColor;
    }

    void LateUpdate()
    {
        float yOffset = body.localScale.y * followStrength;
        transform.localPosition = new Vector3(0, baseHeight + yOffset, 0);
    }
}
