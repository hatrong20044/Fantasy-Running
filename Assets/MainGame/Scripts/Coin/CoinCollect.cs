
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    [SerializeField] private int rotateSpeed = 1;
    [SerializeField] private AudioSource coinFX;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            EventManager.Instance.CoinCollected(gameObject,1);
        }
       
    }
    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
