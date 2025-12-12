using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = .2f;

    private Rigidbody2D rb;
    private PlayerControllerCombined controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerControllerCombined>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        if (rb == null) return;

        // nếu đang bị knockback, có thể reset thời gian (tùy muốn)
        gettingKnockedBack = true;

        // khóa điều khiển nếu có
        if (controller != null)
            controller.enabled = false;

        // reset velocity trước khi đẩy
        rb.velocity = Vector2.zero;

        Vector2 difference = new Vector2(
            transform.position.x - damageSource.position.x,
            transform.position.y - damageSource.position.y
        ).normalized * knockBackThrust;

        rb.AddForce(difference, ForceMode2D.Impulse);

        // bắt đầu coroutine tắt knockback
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);

        if (rb != null)
            rb.velocity = Vector2.zero;

        gettingKnockedBack = false;

        if (controller != null)
            controller.enabled = true;
    }
}
