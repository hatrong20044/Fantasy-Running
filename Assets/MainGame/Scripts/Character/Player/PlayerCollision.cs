using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]private GameObject  canvas;
    public float warningDuration; // 1.0f


    // handle event when player collide obstacle
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "Collision")
        {
            this.handleObstacleCollision(other);
        }
        else if (other.gameObject.name == "Activation")
        {
            RunWarnning runWarning = other.gameObject.GetComponentInParent<RunWarnning>();
            Movement obstacleMovement = other.gameObject.GetComponentInParent<Movement>();
            runWarning.Act();
            StartCoroutine(DelayedStartMoving(obstacleMovement, runWarning.warningDuration));
        }
    }

    private IEnumerator DelayedStartMoving(Movement movement, float delay)
    {
        yield return new WaitForSeconds(delay); // Chờ delay (1 giây)
        movement.StartMoving();
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
