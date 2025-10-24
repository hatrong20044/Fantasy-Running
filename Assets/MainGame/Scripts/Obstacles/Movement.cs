using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    protected bool isMoving = false;
    protected abstract void Move();

    protected virtual void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    // Bắt đầu di chuyển
    public void StartMoving()
    {
        isMoving = true;
    }

    // Dừng di chuyển
    public void ResetMoving()
    {
        isMoving = false;
    }
}
