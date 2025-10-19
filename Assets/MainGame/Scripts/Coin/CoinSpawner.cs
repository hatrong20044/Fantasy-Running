
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Transform cameraTransf;
    [SerializeField] private PatternManager patternManager;
    [SerializeField] private ZoneManager zoneManager;
    [SerializeField] private float lastSpawnZ;
    [SerializeField] private int maxActiveCoins = 30;
    [SerializeField] private List<GameObject> activeCoins;
    [SerializeField] private float randomSpacePattern;
    [SerializeField] private int limitCoins = 10;
    private void Awake()
    {
        activeCoins = new();
        lastSpawnZ = cameraTransf.position.z;
    }

    private void Update()
    {
        this.RecycleCoins();
        this.SpawnRandomPattern();
       
    }

    public void SpawnRandomPattern()
    {
        if (activeCoins.Count + limitCoins >= maxActiveCoins)
        {
            return;
        }
        PatternManager.CoinPattern pattern = patternManager.GetRandomPattern();

        // Tính vị trí Z bắt đầu của pattern
        randomSpacePattern = Random.Range(5f, 50f);
        float startZ = lastSpawnZ + randomSpacePattern;
        Vector3[] patternPositions = pattern.positions;
        Vector3 spawnPos = new Vector3();

        //Spawn coin cho pattern
        foreach(Vector3 pos in patternPositions)
        {
            spawnPos = new Vector3(
                pos.x,
                pos.y,
                pos.z + startZ

            );
            if(zoneManager.CanPlaceCoin(spawnPos))
            {
                GameObject coin = ObjectPool.Instance.GetFromPool("Coin");
                coin.transform.position = spawnPos;
                zoneManager.RegisterCoin(spawnPos);
                activeCoins.Add(coin);
            }
            else
            {
                Debug.Log("khong thay key ");
                return;
            }
            
        }
        lastSpawnZ = spawnPos.z;
    }

    public void SpawnPattern(PatternManager.CoinPattern pattern)
    {

    }
  
    public void SpawnCoinForPattern()
    {

    }

    private void RecycleCoins()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            if (activeCoins[i].activeSelf && activeCoins[i].transform.position.z < cameraTransf.position.z)
            {
                ObjectPool.Instance.ReturnToPool("Coin", activeCoins[i]);
                zoneManager.RegisterCoin(activeCoins[i].transform.position);
                activeCoins.Remove(activeCoins[i]);
                Debug.Log(activeCoins.Count);
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