using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // handle event when player collide obstacle
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Game over");
    }
}
