using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] private ParticleSystem effectPrefab; // Gán qua Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Ki?m tra n?u effectPrefab ???c gán
        if (effectPrefab == null)
        {
            Debug.LogError("effectPrefab is not assigned!");
            return;
        }

        // T?o instance c?a hi?u ?ng n? t?i v? trí bullet
        ParticleSystem effectInstance = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        effectInstance.Play(); // Phát hi?u ?ng

        // T?t bullet sau khi t?o hi?u ?ng
        gameObject.SetActive(false);

        // (Tùy ch?n) H?y hi?u ?ng sau khi hoàn t?t
        Destroy(effectInstance.gameObject, effectInstance.main.duration); // H?y khi hi?u ?ng k?t thúc
    }
}
