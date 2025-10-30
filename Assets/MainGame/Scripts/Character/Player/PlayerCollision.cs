using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    public float warningDuration; // 1.0f
    private Player player;
    private void Start()
    {
        player = this.GetComponent<Player>();
    }
    // handle event when player collide obstacle
    private void OnTriggerEnter(Collider other)
    {
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
        else if(other.gameObject.name == "Skill(Clone)")
        {
            player.CastSkill();
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
        player.Die();
        StartCoroutine(ShowWarning());
    }

    IEnumerator ShowWarning()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ShowUI(UIName.GameOver);
        EventManager.Instance.PlayerCollided(gameObject);
    } ////
}
