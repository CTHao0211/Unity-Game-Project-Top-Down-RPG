using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = .2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        gettingKnockedBack = true;

        // ⭐ CHỈ LẤY X,Y — KHÔNG CHO Z THAM GIA TÍNH LỰC
        Vector2 difference = new Vector2(
            transform.position.x - damageSource.position.x,
            transform.position.y - damageSource.position.y
        ).normalized * knockBackThrust * rb.mass;

        rb.AddForce(difference, ForceMode2D.Impulse);

        // ⭐ GIỮ ENEMY LUÔN Ở Z = 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        gettingKnockedBack = false;

        // ⭐ ĐẢM BẢO SAU KNOCKBACK KHÔNG BỊ LỆCH Z
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
