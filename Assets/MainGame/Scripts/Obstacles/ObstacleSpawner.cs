using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    //public List<GameObject> passableObstacles; // passable obstcles prefab
    //public List<GameObject> nonPassableObstacles; // non passable obstacles prefab
    //public GameObject obstaclePrefab; // obstcale prefab
    //public float distanceObtacle = 10f; // distance between obsatcles when we spawn
    //public int maxSystemObstacle = 10;
    //public float laneDistance = 3.5f; // distance between lanes (Left, Middle, Right)
    //public float currentObstaclePosZ = 10f; // the final position of the system obstacle was spawn
    //[SerializeField] public List<GameObject> obstaclesManager;

    //private void Awake()
    //{
    //    this.obstaclePrefab = GameObject.Find("ObstaclePrefab");
    //}

    //private void Start()
    //{
    //    this.passableObstacles = new List<GameObject>();
    //    this.nonPassableObstacles = new List<GameObject>();
    //    foreach (Transform child in this.obstaclePrefab.transform)
    //    {
    //        foreach (Transform child2 in child.transform)
    //        {
    //            if (child.name == "PassableObstacle")
    //            {
    //                this.passableObstacles.Add(child2.transform.gameObject);
    //            }
    //            else if (child.name == "NonPassableObstacle")
    //            {
    //                this.nonPassableObstacles.Add(child2.transform.gameObject);
    //                Debug.Log(child2.name);
    //            }
    //        }
    //    }

    //    this.spawn();
    //}

    //private void Update()
    //{

    //}

    ////Generate a list obstacles include 2 nonpassable obstacle and 1 passable obstacle
    //public List<GameObject> generateSystemObstacle()
    //{
    //    List<GameObject> obstacles = new List<GameObject>();
    //    int randomIndex = 0;
    //    //create and add 2 random non passable obstacle and add to list
    //    for (int i = 0; i < 2; i++)
    //    {
    //        randomIndex = Random.Range(0, this.nonPassableObstacles.Count);
    //        GameObject nonPassableObstacle = Instantiate(nonPassableObstacles[randomIndex]);
    //        obstacles.Add(nonPassableObstacle);
    //    }
    //    //create a passable obstacle and add to list
    //    randomIndex = Random.Range(0, this.passableObstacles.Count);
    //    GameObject passableObstacle = Instantiate(passableObstacles[randomIndex]);
    //    obstacles.Add(passableObstacle);

    //    return obstacles;
    //}

    //// set position for each obstacle in list obstacle to create a system obstcale
    //// a obstacle system include 2 non passable obstacle and a passable obsatcle that was set on a horizontal row.
    //public void setPosObstacleSystem(List<GameObject> obstacles)
    //{
    //    List<int> lanes = new List<int> {-1, 0, 1};
    //    for (int i = 0; i < obstacles.Count; i++)
    //    {
    //        int laneIndex = Random.Range(0, lanes.Count);
    //        Vector3 position = new Vector3(this.laneDistance * lanes[laneIndex], obstacles[i].transform.position.y, this.currentObstaclePosZ);
    //        lanes.RemoveAt(laneIndex);
    //    }
    //}

    //// spawn obstacle systems
    //public void spawn()
    //{
    //    for (int i = 0; i < this.maxSystemObstacle; i++)
    //    {
    //        List<GameObject> obstacles = this.generateSystemObstacle();
    //        setPosObstacleSystem(obstacles);
    //        this.currentObstaclePosZ += distanceObtacle;
    //        this.obstaclesManager.AddRange(obstacles);
    //    }
    //}

    public List<string> passableObstacleTags;
    public List<string> nonPassableObstacleTags;
    public ObstaclePooler obstaclePooler;
    public GameObject obstaclePrefab; // obstcale prefab
    public float distanceObtacle = 10f; // distance between obsatcles when we spawn
    public int maxSystemObstacle = 10;
    public float laneDistance = 3.5f; // distance between lanes (Left, Middle, Right)
    public float currentObstaclePosZ = 10f; // the final position of the system obstacle was spawn
    public float currentResetPosZ = 15f;
    public GameObject Player;

    private void Awake()
    {
        this.Player = GameObject.Find("Player");
        this.obstaclePrefab = GameObject.Find("ObstaclePrefab");
        this.obstaclePooler = FindObjectOfType<ObstaclePooler>();
        foreach (Transform child in this.obstaclePrefab.transform)
        {
            foreach (Transform child2 in child.transform)
            {
                Debug.Log(child2.name);
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

    private void Start()
    {
        this.spawn();
    }

    private void Update()
    {
        this.resetObstacle();
    }

    //Generate a list obstacles include 2 nonpassable obstacle and 1 passable obstacle
    public List<GameObject> generateSystemObstacle()
    {
        List<GameObject> obstacles = new List<GameObject>();
        int randomIndex = 0;
        //create and add 2 random non passable obstacle and add to list
        for (int i = 0; i < 2; i++)
        {
            randomIndex = Random.Range(0, this.nonPassableObstacleTags.Count);
            GameObject nonPassableObstacle = this.obstaclePooler.getObstacle(this.nonPassableObstacleTags[randomIndex]);
            obstacles.Add(nonPassableObstacle);
        }
        //create a passable obstacle and add to list
        randomIndex = Random.Range(0, this.passableObstacleTags.Count);
        GameObject passableObstacle = this.obstaclePooler.getObstacle(this.passableObstacleTags[randomIndex]);
        obstacles.Add(passableObstacle);

        return obstacles;
    }

    // set position for each obstacle in list obstacle to create a system obstcale
    // a obstacle system include 2 non passable obstacle and a passable obsatcle that was set on a horizontal row.
    public void setPosObstacleSystem(List<GameObject> obstacles)
    {
        List<int> lanes = new List<int> { -1, 0, 1 };
        for (int i = 0; i < obstacles.Count; i++)
        {
            int laneIndex = Random.Range(0, lanes.Count);
            obstacles[i].transform.position = new Vector3(this.laneDistance * lanes[laneIndex], obstacles[i].transform.position.y, this.currentObstaclePosZ);
            lanes.RemoveAt(laneIndex);
        }
    }

    // spawn obstacle systems when start game
    public void spawn()
    {
        for (int i = 0; i < this.maxSystemObstacle; i++)
        {
            List<GameObject> obstacles = this.generateSystemObstacle();
            setPosObstacleSystem(obstacles);
            this.currentObstaclePosZ += distanceObtacle;
        }
    }

    // reuse the obstacle systems that player overcame
    public void resetObstacle()
    {
        if (this.Player.transform.position.z > this.currentResetPosZ)
        {
            List<GameObject> obstacles = this.generateSystemObstacle();
            setPosObstacleSystem(obstacles);
            this.currentResetPosZ += this.distanceObtacle;
            this.currentObstaclePosZ += distanceObtacle;
        }
    }
}
