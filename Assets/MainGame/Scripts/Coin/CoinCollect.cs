
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    [SerializeField] private int rotateSpeed = 1;
    [SerializeField] private AudioSource coinFX;
    [SerializeField] private ParticleSystem collectEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectEffect.transform.SetParent(null);
            collectEffect.Play();
            EventManager.Instance.CoinCollected(gameObject);
        }
       
    }
    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
