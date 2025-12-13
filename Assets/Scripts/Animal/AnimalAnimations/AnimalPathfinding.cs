using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;

    private void Awake()
    {
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (knockback.gettingKnockedBack) return;

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        float speed = moveDir.magnitude;
        GetComponent<AnimalAudio>()?.PlayMovementSound(speed);
    }


    public void MoveTo(Vector2 targetPosition)
    {
        // === targetPosition là TỌA ĐỘ, phải chuyển thành HƯỚNG ===    
        moveDir = (targetPosition - rb.position).normalized;
    }

    public Vector2 GetMoveDir()
    {
        return moveDir;
    }
}
