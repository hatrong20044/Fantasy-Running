
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
            ParticleSystem collectEffectInstance = Instantiate(collectEffect, transform.position, Quaternion.identity);
            collectEffectInstance.Play();
            EventManager.Instance.CoinCollected(gameObject);
            Destroy(collectEffectInstance.gameObject, collectEffectInstance.main.duration);
        }
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
