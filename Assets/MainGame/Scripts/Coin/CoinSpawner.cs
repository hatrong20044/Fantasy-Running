
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private PatternManager patternManager; 
    [SerializeField] private Vector3 lastSpawn;
   [SerializeField] private float destroyDistance = 2f;
    private List<GameObject> activeCoins = new();

    private void Start()
    {
        SpawnCoins(player.position.z + 20f);
    }

    private void Update()
    {
        this.CheckandSpawnCoin();
        // this.DestroyCoin();
    }

    // Spawn coin moi khi player di chuyen du khoang cach
    public void CheckandSpawnCoin()
    {
        float x = patternManager.SpawnRandomDistance();
        float zPosCur = lastSpawn.z + x;
        if (player.position.z >= zPosCur )
        {
            while(activeCoins.Count <= 30)
            {
                SpawnCoins(zPosCur + 10f);
            }
            
        }
    }
    //Thu hoi Coin khi di qua
    public void DestroyCoin()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            GameObject coin = activeCoins[i];
            if (coin != null && coin.activeSelf && coin.transform.position.z < player.position.z - destroyDistance)
            {
                ObjectPool.Instance.ReturnToPool("Coin", coin);
                activeCoins.RemoveAt(i);
            }
        }
    }

    private void SpawnCoins(float zOffset)
    {

        PatternManager.CoinPattern pattern = patternManager.GetRandomPattern();
        foreach(Vector3 pos in pattern.positions)
        {
            Vector3 coinPosition = new Vector3(
                pos.x, pos.y, pos.z + zOffset
            );
            GameObject coin = ObjectPool.Instance.GetFromPool("Coin");
            if( coin != null )
            {
                coin.transform.SetPositionAndRotation(coinPosition, Quaternion.identity);
                activeCoins.Add(coin);
                lastSpawn = coinPosition;
            }
        }
    }

    private void Reset()
    {
        this.LoadComponents();
    }

    protected void LoadComponents()
    {
        this.LoadPatternManager();
    }

    protected void LoadPatternManager()
    {
        this.patternManager = transform.GetComponent<PatternManager>();
    }


    

}