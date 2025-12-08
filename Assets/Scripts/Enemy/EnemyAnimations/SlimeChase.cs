using UnityEngine;

public class SlimeChase : MonoBehaviour
{
    private Knockback knockback;

    [Header("Target")]
    public Transform target;           // có thể kéo Player, hoặc để trống

    [Header("Chase Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.8f;  // đứng cách player 1 đoạn

    [Header("Aggro Settings")]
    public float aggroRange = 5f;      // khoảng cách bắt đầu dí
    public float loseRange = 7f;       // chạy xa hơn mức này thì slime bỏ cuộc
    public float returnSpeed = 2f;     // tốc độ về vị trí ban đầu

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 spawnPosition;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();

        // ⭐ LƯU VỊ TRÍ GỐC CỦA TỪNG CON SLIME
        spawnPosition = transform.position;
    }

    private void Start()
    {
        // ⭐ NẾU CHƯA GÁN TARGET TRONG INSPECTOR → TỰ TÌM PLAYER THEO TAG
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                target = p.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null || rb == null) return;

        // 🔴 đang bị knockback → không điều khiển di chuyển
        if (knockback != null && knockback.gettingKnockedBack)
        {
            if (animator != null)
                animator.SetBool("IsMoving", false);
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, target.position);

        // 1) Quyết định có CHASE hay không
        if (!isChasing && distToPlayer <= aggroRange)
        {
            isChasing = true;
        }
        else if (isChasing && distToPlayer >= loseRange)
        {
            isChasing = false;
        }

        // 2) Hành vi khi CHASING / RETURN
        if (isChasing)
            ChaseTarget(distToPlayer);
        else
            ReturnToSpawn();
    }

    private void ChaseTarget(float distToPlayer)
    {
        Vector2 dir = (target.position - transform.position);

        if (distToPlayer > stopDistance)
        {
            dir.Normalize();
            rb.velocity = dir * moveSpeed;

            if (animator != null)
                animator.SetBool("IsMoving", true);
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (animator != null)
                animator.SetBool("IsMoving", false);
        }
    }

    private void ReturnToSpawn()
    {
        Vector2 dir = (spawnPosition - (Vector2)transform.position);
        float dist = dir.magnitude;

        if (dist > 0.05f)
        {
            dir.Normalize();
            rb.velocity = dir * returnSpeed;

            if (animator != null)
                animator.SetBool("IsMoving", true);
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (animator != null)
                animator.SetBool("IsMoving", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
