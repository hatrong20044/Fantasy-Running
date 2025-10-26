
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ZoneManager zoneManager;
    [SerializeField] private CoinSpawner coinSpawner;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
   // [SerializeField] private PatternManager patternManager;
    [SerializeField] private Transform cameraTransf;
    [SerializeField] private SkillSpawner skillSpawner;

    private void Update()
    {
        coinSpawner.SpawnRandomPattern();
        obstacleSpawner.ResetObstacle(cameraTransf.position.z);
        if (!obstacleSpawner.checkSpawnCondition() && BossManager.instance.nextBoss.persistUntilDefeated)
        {
            skillSpawner.ResetSkill(cameraTransf.position.z);
        }
        zoneManager.UpdateWithCameraPosition(cameraTransf.position.z);
    }

    private void Reset()
    {
        this.LoadComponents();
    }

    private void LoadComponents()
    {
        this.LoadZoneManager();
        this.LoadCoinSpawner();
        this.LoadObstacleSpawner();
       // this.LoadPatternManager();
    }

    protected void LoadZoneManager()
    {
        this.zoneManager = transform.GetComponent<ZoneManager>();
    }

    protected void LoadCoinSpawner()
    {
        this.coinSpawner = transform.GetComponent<CoinSpawner>();
    }

    protected void LoadObstacleSpawner()
    {
        this.obstacleSpawner = transform.GetComponent<ObstacleSpawner>();
    }
    protected void LoadPatternManager()
    {
        //this.patternManager = transform.GetComponent<PatternManager>();
    }

    public Vector3 GetCameraTransform()
    {
        return this.cameraTransf.position;
    } 
}
