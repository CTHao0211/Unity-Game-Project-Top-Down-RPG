using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private SpriteRenderer spriteRenderer;

    // Phạm vi đi lang thang (tính từ vị trí ban đầu)
    [SerializeField] private float roamRadius = 3f;

    private Vector2 startPosition;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = State.Roaming;
        startPosition = transform.position;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            // Lấy vị trí random quanh chỗ spawn
            Vector2 roamPosition = GetRoamingPosition();

            // Bảo con thú đi tới đó
            enemyPathfinding.MoveTo(roamPosition);

            // Lấy hướng thực tế
            Vector2 dir = enemyPathfinding.GetMoveDir();

            // Xoay trái/phải theo trục X
            if (dir.x < 0)
                spriteRenderer.flipX = true;   // quay sang trái
            else if (dir.x > 0)
                spriteRenderer.flipX = false;  // quay sang phải

            // Đợi 2 giây rồi random chỗ mới
            yield return new WaitForSeconds(2f);
        }
    }

    // Random vị trí trong vòng tròn quanh vị trí ban đầu
    private Vector2 GetRoamingPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        return startPosition + randomOffset;
    }
}
