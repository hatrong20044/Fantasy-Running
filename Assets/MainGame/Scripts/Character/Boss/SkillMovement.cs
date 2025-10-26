using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillMovement : Movement
{
    public float speed = 25f;
    [SerializeField] private float heightOffset = 8f;   // Độ cao của boss so với người chơi
    [SerializeField] private float distanceOffset = 35f;
    private Vector3 targetPosition;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    protected override void Move()
    {
        targetPosition = player.transform.position + new Vector3(0, heightOffset, distanceOffset);
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
