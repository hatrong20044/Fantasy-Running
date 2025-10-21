
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

  
    void Start()
    {
        CoinEvent.Instance.OnCoinCollected += PlayCoinSound;
    }

    private void OnDestroy()
    {
        CoinEvent.Instance.OnCoinCollected -= PlayCoinSound;
    }

    public void PlayCoinSound(GameObject coin)
    {
        if(audioSource == null) { Debug.Log("aaaaaa"); }
        audioSource.Play();
    }
}
