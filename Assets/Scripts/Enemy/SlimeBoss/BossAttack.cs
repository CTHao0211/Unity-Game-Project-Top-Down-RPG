using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public int skillDamage = 2;
    public float attackRange = 1.1f;
    public float attackCooldown = 1.5f;

    public Transform attackPoint;
    public LayerMask playerLayer;

    private Animator animator;
    private bool isAttacking;
    private float lastAttackTime;
    private CrownFollow crown;
    private PlayerHealth currentPlayer;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        crown = GetComponentInChildren<CrownFollow>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            currentPlayer = collision.collider.GetComponent<PlayerHealth>();
            TryAttack();
        }
    }


    void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator?.SetTrigger("Skill");

        if (crown != null)
            crown.OnBossAttack();

        yield return new WaitForSeconds(0.3f);
        DoDamage();

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    private void DoDamage()
    {
        if (currentPlayer == null) return;

        currentPlayer.TakeDamage(
            skillDamage,
            transform,
            Color.red,
            true,   // knockback
            true    // ignore invuln
        );
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            currentPlayer = null;
        }
    }

}
