using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool isInvincible = false;

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
        Debug.Log("Game over");
    }
}
