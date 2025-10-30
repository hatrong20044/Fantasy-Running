using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectSound : MonoBehaviour
{
    [SerializeField] private AudioSource coinFX;
    private void Start()
    {
        EventManager.Instance.OnCoinCollected += PlayingSound;
    }
    private void OnDestroy()
    {
        EventManager.Instance.OnCoinCollected -= PlayingSound;

    }

    public void PlayingSound(GameObject coin)
    {
        coinFX.Play();
    }
   
}
