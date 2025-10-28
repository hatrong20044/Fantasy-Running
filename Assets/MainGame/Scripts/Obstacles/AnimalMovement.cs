using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMovement : Movement
{
    [SerializeField] public float moveSpeed = 20f;
    [SerializeField] private Animator anim;
    private string currentAnim;

    protected override void Move()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        ChangeAnim("Run");
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;
        currentAnim = animName;
        anim.CrossFadeInFixedTime(animName, 0.1f);
    }
}
