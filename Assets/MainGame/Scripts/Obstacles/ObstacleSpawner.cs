using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public List<string> passableObstacleTags; // include the name of passable obstacles
    public List<string> nonPassableObstacleTags; // include the name of non passable obstacles
    public ObstaclePooler obstaclePooler; // Use object pooling to spawn obstacle systems
    public GameObject obstaclePrefab; // obstcale prefab
    public float distanceObtacle = 10f; // distance between obsatcles when we spawn
    public int maxSystemObstacle = 10; // max obstacle system we have
    public float laneDistance = 2.5f; // distance between lanes (Left, Middle, Right)
    public float currentObstaclePosZ = 10f; // the final position of the system obstacle was spawn
    public float currentResetPosZ = 15f; // if player overcome this position, reuse obsctacle system and spawn 
    public string curentSeason;
    public GameObject Player;

    private void Awake()
    {
        this.Player = GameObject.Find("Player");
        this.obstaclePrefab = GameObject.Find("ObstaclePrefab");
        this.obstaclePooler = FindObjectOfType<ObstaclePooler>();
        this.curentSeason = "Summer";
        AssetCollector.instance.LoadSeason(this.curentSeason);
    }

    private void Start()
    {
        this.Init();
        this.Spawn();
    }

    private void Update()
    {
        this.ResetObstacle();
    }

    // get all kind of obstacles and add them to obstaclePools to prepare to g
    public void Init()
    {
        foreach (Transform child in this.obstaclePrefab.transform)
        {
            foreach (Transform child2 in child.transform)
            {
                ObstaclePool obstaclePool = null;
                if (child.name == "PassableObstacle")
                {
                    this.passableObstacleTags.Add(child2.name);
                    obstaclePool = new ObstaclePool(child2.name, child2.gameObject, maxSystemObstacle);
                }
                else if (child.name == "NonPassableObstacle")
                {
                    this.nonPassableObstacleTags.Add(child2.name);
                    // because of a obstacle system include 2 non passable obstacle and a passable obstacle so we have to create pool with size is twice as maxSystemObstacle
                    obstaclePool = new ObstaclePool(child2.name, child2.gameObject, maxSystemObstacle * 2);
                }
                if (obstaclePool != null)
                {
                    this.obstaclePooler.obstaclePools.Add(obstaclePool);
                }
            }
        }

        this.obstaclePooler.generatePool();
    }

    //Generate a list obstacles include 2 nonpassable obstacle and 1 passable obstacle
    public List<GameObject> GenerateSystemObstacle()
    {
        List<GameObject> obstacles = new List<GameObject>();
        int randomBaseObstacle = 0;
        int randomApperanceObstacle = 0;
        //create and add 2 random non passable obstacle and add to list
        for (int i = 0; i < 2; i++)
        {
            randomBaseObstacle = UnityEngine.Random.Range(0, this.nonPassableObstacleTags.Count);
            randomApperanceObstacle = UnityEngine.Random.Range(0, AssetCollector.instance.currentNonPassable.Count);
            GameObject nonPassableObstacle = this.obstaclePooler.getObstacle(this.nonPassableObstacleTags[randomBaseObstacle]);
            this.ApplyMeshAndMeshRenderer(nonPassableObstacle, randomApperanceObstacle, AssetCollector.instance.currentNonPassable);
            obstacles.Add(nonPassableObstacle);
        }
        //create a passable obstacle and add to list
        randomBaseObstacle = UnityEngine.Random.Range(0, this.passableObstacleTags.Count);
        randomApperanceObstacle = UnityEngine.Random.Range(0, AssetCollector.instance.currentPassable.Count);
        GameObject passableObstacle = this.obstaclePooler.getObstacle(this.passableObstacleTags[randomBaseObstacle]);
        this.ApplyMeshAndMeshRenderer(passableObstacle, randomApperanceObstacle, AssetCollector.instance.currentPassable);
        obstacles.Add(passableObstacle);

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
        filter2.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        filter2.sharedMesh = obstacleAssets[randomIndex].prefab.GetComponent<MeshFilter>().mesh;
        meshRenderer2.sharedMaterials = obstacleAssets[randomIndex].prefab.GetComponent<MeshRenderer>().materials;
    }

    // spawn obstacle systems when start game
    public void Spawn()
    {
        for (int i = 0; i < this.maxSystemObstacle; i++)
        {
            List<GameObject> obstacles = this.GenerateSystemObstacle();
            SetPosObstacleSystem(obstacles);
            this.currentObstaclePosZ += distanceObtacle;
        }
    }

    // reuse the obstacle systems that player overcame
    public void ResetObstacle()
    {
        if (this.Player.transform.position.z > this.currentResetPosZ)
        {
            List<GameObject> obstacles = this.GenerateSystemObstacle();
            SetPosObstacleSystem(obstacles);
            this.currentResetPosZ += this.distanceObtacle;
            this.currentObstaclePosZ += distanceObtacle;
        }
    }
}
