using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] private AudioSource coinFX;
    [SerializeField] private int coin = 0;
    

    //A callback method (special method) that is automatically called when a Collider
    //marked as a Trigger collides with another Collider.”
    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        this.gameObject.SetActive(false);
        this.UpdateCoin();


    }

    protected void UpdateCoin()
    {
        coin++;
    }
    

}
