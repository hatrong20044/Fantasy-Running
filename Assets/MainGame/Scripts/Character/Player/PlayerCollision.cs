using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    private bool isInvincible = false;
    [SerializeField]private GameObject  canvas;

    private void OnCollisionEnter(Collision collision)
    {
        this.GameOver();
    }

    // handle event when player collide obstacle
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collision")
        {
            this.handleObstacleCollision(other);
        }
    }

    public void handleObstacleCollision(Collider Collider)
    {
        this.GameOver();
    }

    public void GameOver()
    {
        canvas.SetActive(true);
    }
}
