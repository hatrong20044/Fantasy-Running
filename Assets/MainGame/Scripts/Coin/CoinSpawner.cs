
using UnityEngine;


public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private PatternManager patternManager;
    [SerializeField] private ZoneManager zoneManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float lastSpawnZ;
    [SerializeField] private int maxActiveCoins = 30;
    [SerializeField] private float randomSpacePattern;
    [SerializeField] private int limitCoins = 10;
    private void Awake()
    {
        lastSpawnZ = levelManager.GetCameraTransform().z;
    }
    private void Start()
    {
        CoinEvent.Instance.OnCoinCollected += coin => HandleCoinCollected("Coin", coin);

    }

    private void OnDestroy()
    {
        CoinEvent.Instance.OnCoinCollected -= coin => HandleCoinCollected("Coin", coin);

    }

    public void SpawnRandomPattern()
    {
        if (ObjectPool.Instance.GetActiveCount("Coin") + limitCoins >= maxActiveCoins)
        {
            return;
        }
        PatternManager.CoinPattern pattern = patternManager.GetRandomPattern();
        // Tính vị trí Z bắt đầu của pattern
        randomSpacePattern = Random.Range(25f, 50f);
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
                GameObject coin = ObjectPool.Instance.GetFromPoolQuynh("Coin");
                coin.transform.position = spawnPos;
                zoneManager.RegisterCoin(spawnPos);
            }
            else
            {
               
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

    public void HandleCoinCollected(string tag, GameObject coin)
    {
        ObjectPool.Instance.ReturnToPoolQuynh("Coin", coin);
    }

    private void Reset()
    {
        this.LoadComponents();
    }

    protected void LoadComponents()
    {
        this.LoadPatternManager();
        this.LoadLevelManager();
        this.LoadZoneManager();
    }

    protected void LoadPatternManager()
    {
        this.patternManager = transform.GetComponent<PatternManager>();
    }
    protected void LoadLevelManager()
    {
        this.levelManager = transform.GetComponent<LevelManager>();
    }

    protected void LoadZoneManager()
    {
        this.zoneManager = transform.GetComponent<ZoneManager>();
    }

}