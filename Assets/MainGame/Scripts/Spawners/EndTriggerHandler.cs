using UnityEngine;

public class EndTriggerHandler : MonoBehaviour
{
    private GroundSpawner spawner;
    private string mapTag;
    private GameObject tile;

    public void Init(GroundSpawner spawner, string mapTag, GameObject tile)
    {
        this.spawner = spawner;
        this.mapTag = mapTag;
        this.tile = tile;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Giả sử player có tag "Player"
        {
            spawner.SpawnTile();
            // Có thể thêm logic để trả map về pool nếu cần
            ObjectPool.Instance.ReturnToPool(mapTag, tile);
        }
    }
}