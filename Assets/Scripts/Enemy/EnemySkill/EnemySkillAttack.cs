using System.Collections;
using UnityEngine;

public class EnemySkillAttack : MonoBehaviour
{
    [Header("Skill Settings")]
    public int skillDamage = 2;
    public float attackRange = 1.1f;
    public float attackCooldown = 1.5f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask playerLayer;

    private Animator animator;
    private bool isAttacking;
    private float lastAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (!isAttacking && dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator?.SetTrigger("Skill");

        yield return new WaitForSeconds(0.3f);
        DoDamage();

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    private void DoDamage()
    {
        if (attackPoint == null) attackPoint = transform;

        // Quét mọi collider trong vòng tròn, không lọc layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            // Chỉ xử lý nếu là Player
            if (hit.CompareTag("Player"))
            {
                Debug.Log("[Slime] Đánh trúng Player!");

                HealthBase hp = hit.GetComponent<HealthBase>(); // hoặc PlayerHealth nếu bạn tách riêng
                if (hp != null)
                {
                    hp.TakeDamage(skillDamage, transform, Color.red);
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
