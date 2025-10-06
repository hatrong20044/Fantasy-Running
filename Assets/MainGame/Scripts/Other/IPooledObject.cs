using UnityEngine;

public interface IPooledObject
{
    void OnSpawn(); // Gọi khi đối tượng được spawn
    void OnReturn(); // Gọi khi đối tượng được trả về pool
}
