using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectSound : MonoBehaviour
{
    [SerializeField] private AudioSource coinFX;
    private void Start()
    {
        CoinEvent.Instance.OnCoinCollected += PlayingSound;
    }
    private void OnDestroy()
    {
        CoinEvent.Instance.OnCoinCollected -= PlayingSound;

    }

    public void PlayingSound(GameObject coin)
    {
        coinFX.Play();
    }
    private void Reset()
    {
        this.coinFX = GetComponent<AudioSource>();
    }
}
