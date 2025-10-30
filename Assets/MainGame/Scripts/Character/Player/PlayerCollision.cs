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
            float timeWarning = (4 / 7f)*(30 / player.forwardSpeed);
            Debug.Log(timeWarning + " " + 30/player.forwardSpeed);
            RunWarnning runWarning = other.gameObject.GetComponentInParent<RunWarnning>();
            Movement obstacleMovement = other.gameObject.GetComponentInParent<Movement>();
            runWarning.SetTimeWarning(timeWarning);
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
        yield return new WaitForSeconds(delay);
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
<<<<<<< HEAD
        EventManager.Instance.PlayerCollided(gameObject);
    } ////
=======
        //canvas.SetActive(true);
    }
>>>>>>> 95f33d4af547a6379a833ca5e06c531fa933a318
}
