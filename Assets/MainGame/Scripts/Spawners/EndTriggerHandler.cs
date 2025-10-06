using UnityEngine;

public class EndTriggerHandler : MonoBehaviour
{
    private GroundSpawner spawner;
    private string poolTag;
    private GameObject mapTile;

    public void Init(GroundSpawner spawner, string poolTag, GameObject mapTile)
    {
        this.spawner = spawner;
        this.poolTag = poolTag;
        this.mapTile = mapTile;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Spawn map mới
            spawner.SpawnTile();

            // Return map cũ về pool
            ObjectPool.Instance.ReturnToPool(poolTag, mapTile);

            // Xoá handler để không chạy lại
            Destroy(this);
        }
    }
}
