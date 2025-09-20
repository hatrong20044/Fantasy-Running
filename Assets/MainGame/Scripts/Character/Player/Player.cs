using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Lấy input từ mũi tên
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");     

        Vector3 move = new Vector3(horizontal, 0, vertical).normalized;

        // Nếu có input → di chuyển
        if (move.magnitude > 0f)
        {
            // di chuyển theo hướng camera nhìn
            controller.Move(move * moveSpeed * Time.deltaTime);

            // xoay nhân vật về hướng di chuyển
            transform.forward = move;

            // bật animation chạy
            animator.SetBool("Run", true);
        }
        else
        {
            // bật animation idle
            animator.SetBool("Run", false);
        }
    }
}
