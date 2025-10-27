using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : Movement
{

    [SerializeField] private float moveSpeed = 30f; // Tốc độ rơi của tên lửa

    protected override void Move()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
