
using UnityEngine;

public class CoinManager : MonoBehaviour, IPooledObject
{
    [SerializeField] private int rotateSpeed = 1;
    [SerializeField] private AudioSource coinFX;
    [SerializeField] private int coin = 0;

    public void OnSpawn()
    {

    }
    public void OnReturn()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        ObjectPool.Instance.ReturnToPool("Coin", gameObject);
        this.UpdateCoin();
    }

    public void UpdateCoin()
    {
        coin++;
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
