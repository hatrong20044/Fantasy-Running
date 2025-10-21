using UnityEngine;

public class EndTriggerHandler : MonoBehaviour
{
    private GroundSpawner spawner;
    private string mapTag;
    private GameObject tile;
    private bool hasTriggered = false;

    public void Init(GroundSpawner spawner, string mapTag, GameObject tile)
    {
        this.spawner = spawner;
        this.mapTag = mapTag;
        this.tile = tile;
        this.hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            spawner.SpawnTile();

            if (mapTag != "startMap")
            {
                ObjectPool.Instance.ReturnToPool(mapTag, tile);
            }
        }
    }

    private void OnDisable()
    {
        hasTriggered = false;
    }

    public string GetMapTag()
    {
        return mapTag;
    }
}