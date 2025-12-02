﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    // Cho script khác (ví dụ Sword) đọc hướng di chuyển
    public Vector2 Movement => movement;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        AdjustPlayerFacingDirection();   // cho vào đây là hợp lý hơn
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        // Đọc input từ Input System
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // Đẩy vào Animator để điều khiển blend tree / anim chạy
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        // Nếu đang đứng yên thì giữ hướng cũ
        if (movement.sqrMagnitude < 0.001f)
            return;

        // Chỉ flip theo trục X
        if (movement.x < 0f)
        {
            mySpriteRender.flipX = true;   // nhìn sang trái
        }
        else if (movement.x > 0f)
        {
            mySpriteRender.flipX = false;  // nhìn sang phải
        }

        // Nếu bạn muốn nhìn lên/xuống bằng anim (đã set moveY trong Animator)
        // thì không cần xử lý thêm ở đây.
    }
}