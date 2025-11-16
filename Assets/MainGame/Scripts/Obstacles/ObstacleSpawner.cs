
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstaclePosition
{
    public Vector3 Position; // Vị trí của obstacle
    public ObstacleType Type; // Loại obstacle

    // hàm khởi tạo
    public ObstaclePosition(Vector3 position, ObstacleType type)
    {
        Position = position;
        Type = type;
    }
}

public class ObstacleSpawner : MonoBehaviour
{
    public List<string> passableObstacleTags; // include the name of passable obstacles
    public List<string> nonPassableObstacleTags; // include the name of non passable obstacles
    public GameObject obstaclePrefab; // obstcale prefab
    public ZoneManager zoneManager;
    public float minDistanceObstacle = 30f;
    public float maxDistanceObstacle = 50f;
    public float distanceObtacle = 30f; // distance between obsatcles when we spawn
    public int maxSystemObstacle = 10; // max obstacle system we have
    public float laneDistance = 2.5f; // distance between lanes (Left, Middle, Right)
    public float currentObstaclePosZ = 50f; // the final position of the system obstacle was spawn
    public float currentResetPosZ = 55f; // if player overcome this position, reuse obsctacle system and spawn 
    public float destroyDistance = 5f;
    public string curentSeason;
    public BossManager bossManager;

    private void Awake()
    {
        this.zoneManager = transform.GetComponent<ZoneManager>();
        this.curentSeason = "Summer";
    }

    IEnumerator Start()
    {
        yield return null;
        AssetCollector.instance.LoadSeason(this.curentSeason);
        this.Spawn();
    }

    //Generate a list obstacles include 2 nonpassable obstacle and 1 passable obstacle
    public List<GameObject> GenerateSystemObstacle()
    {
        List<GameObject> obstacles = new List<GameObject>();
        int randomNumNonPassObs = Random.Range(1, 3);
        int randomNumPassObs = 3 - randomNumNonPassObs;
        int randomBaseObstacle = 0;
        int randomApperanceObstacle = 0;
        //create and random non passable obstacle and add to list
        for (int i = 0; i < randomNumNonPassObs; i++)
        {
            randomBaseObstacle = UnityEngine.Random.Range(0, this.nonPassableObstacleTags.Count);
            GameObject nonPassableObstacle = ObjectPool.Instance.GetFromPoolQuynh(this.nonPassableObstacleTags[randomBaseObstacle]);

            if (nonPassableObstacle.tag != "Unmeshable")
            {
                ObstacleType nonPassabeObstacleType = nonPassableObstacle.GetComponent<ObstacleType>();
                List<ObstacleAsset> nonPassableObstacleAssets = AssetCollector.instance.GetAssetsBySubType(false, nonPassabeObstacleType.subType);
                randomApperanceObstacle = UnityEngine.Random.Range(0, nonPassableObstacleAssets.Count);
                this.ApplyMeshAndMeshRenderer(nonPassableObstacle.transform.Find("Appearance").gameObject, randomApperanceObstacle, nonPassableObstacleAssets);
            }
            obstacles.Add(nonPassableObstacle);
        }
        //create and random passable obstacle and add to list
        for(int i = 0; i < randomNumPassObs; i++)
        {
            randomBaseObstacle = UnityEngine.Random.Range(0, this.passableObstacleTags.Count);
            GameObject passableObstacle = ObjectPool.Instance.GetFromPoolQuynh(this.passableObstacleTags[randomBaseObstacle]);
            ObstacleType passableObstacleType = passableObstacle.GetComponent<ObstacleType>();
            if (passableObstacleType.subType != ObstacleType.ObstacleSubType.Empty)
            {
                List<ObstacleAsset> passableObstacleAssets = AssetCollector.instance.GetAssetsBySubType(true, passableObstacleType.subType);
                randomApperanceObstacle = UnityEngine.Random.Range(0, passableObstacleAssets.Count);
                this.ApplyMeshAndMeshRenderer(passableObstacle.transform.Find("Appearance").gameObject, randomApperanceObstacle, passableObstacleAssets);
            }
            obstacles.Add(passableObstacle);
        }

        return obstacles;
    }

    // set position for each obstacle in list obstacle to create a system obstcale
    // a obstacle system include 2 non passable obstacle and a passable obsatcle that was set on a horizontal row.
    public void SetPosObstacleSystem(List<GameObject> obstacles)
    {
        List<int> lanes = new List<int> { -1, 0, 1 };
        for (int i = 0; i < obstacles.Count; i++)
        {
            int laneIndex = UnityEngine.Random.Range(0, lanes.Count);
            obstacles[i].transform.position = new Vector3(this.laneDistance * lanes[laneIndex], obstacles[i].transform.position.y, this.currentObstaclePosZ);
            lanes.RemoveAt(laneIndex);
            ObstaclePosition obstaclePos = new(obstacles[i].transform.position, obstacles[i].GetComponent<ObstacleType>()); // chinh sua
            zoneManager.RegisterObstacle(obstaclePos); // dang ki voi ZoneManager
        }
    }

    // apply prefab's mesh and prefab's mesh renderer to obstacle (apply apperance)
    public void ApplyMeshAndMeshRenderer(GameObject gameObject, int randomIndex, List<ObstacleAsset> obstacleAssets)
    {
        MeshFilter filter2 = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer2 = gameObject.GetComponent<MeshRenderer>();
        if (filter2 == null && meshRenderer2 == null) return;

        filter2.transform.localScale = obstacleAssets[randomIndex].prefab.transform.localScale;
        filter2.transform.rotation = obstacleAssets[randomIndex].prefab.transform.rotation;
        filter2.transform.position = gameObject.transform.parent.position;

        filter2.sharedMesh = obstacleAssets[randomIndex].prefab.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer2.sharedMaterials = obstacleAssets[randomIndex].prefab.GetComponent<MeshRenderer>().sharedMaterials;
    }

    // spawn obstacle systems when start game
    public void Spawn()
    {
        for (int i = 0; i < this.maxSystemObstacle; i++)
        {
            if (checkSpawnCondition())
            {
                List<GameObject> obstacles = this.GenerateSystemObstacle();
                SetPosObstacleSystem(obstacles);
            }
            this.distanceObtacle = Random.Range(this.minDistanceObstacle, this.maxDistanceObstacle);
            this.currentObstaclePosZ += this.distanceObtacle;
        }
    }

    // reuse the obstacle systems that player overcame
    public void ResetObstacle(float CameraZ)
    {
        if (CameraZ > this.currentResetPosZ)
        {
            if (checkSpawnCondition())
            {
                List<GameObject> obstacles = this.GenerateSystemObstacle();
                SetPosObstacleSystem(obstacles);
            }
            this.distanceObtacle = Random.Range(this.minDistanceObstacle, this.maxDistanceObstacle);
            this.currentResetPosZ += this.distanceObtacle;
            this.currentObstaclePosZ += this.distanceObtacle;
        }
    }

    public bool checkSpawnCondition()
    {
        if(bossManager == null) return true;
        if (bossManager.nextBoss != null)
        {
            if (bossManager.nextBoss.bossPrefab.tag == "BossThoiMien")
            {
                return true;
            }

            if (bossManager.isSpawningOrActive)
            {
                return false; // Không spawn obstacle nếu boss đang hoạt động
            }

            float bossTriggerDistance = bossManager.triggerDistance;
            if (currentObstaclePosZ > bossTriggerDistance)
            {
                return false;
            }
        }

        return true;
    }
}