
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

  
    void Start()
    {
        EventManager.Instance.OnCoinCollected += PlayCoinSound;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnCoinCollected -= PlayCoinSound;
    }

    public void PlayCoinSound(GameObject coin)
    {
        if(audioSource == null) { Debug.Log("aaaaaa"); }
        audioSource.Play();
    }
}
