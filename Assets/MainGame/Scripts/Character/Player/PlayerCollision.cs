using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    private bool isInvincible = false;
    [SerializeField]private GameObject  canvas;


    // handle event when player collide obstacle
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "Collision")
        {
            //this.handleObstacleCollision(other);
        }
        else if (other.gameObject.name == "Activation")
        {
            ObstacleMovement obstacleMovement = other.GetComponentInParent<ObstacleMovement>();
            Debug.Log(obstacleMovement.gameObject.name);
            if (obstacleMovement != null)
            {
                obstacleMovement.Act();
            }
            else
            {
                Debug.LogError("ObstacleMovement is null for " + other.gameObject.name);
            }
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
