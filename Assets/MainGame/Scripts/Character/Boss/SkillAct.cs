using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillAct : MonoBehaviour
{
    [SerializeField] private SkillMovement skillmovement;
    [SerializeField] private BossEnemy bossEnemy;
    [SerializeField] private ParticleSystem effectPrefab;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            SkillSpawner.Instance.increaseItemUsed();
            skillmovement.StartMoving();
        }
        else if (other.gameObject.name == "Boss")
        {
            bossEnemy.OnTakeDamage(1);
            OnTakeDamageAnimation();
        }
    }

    public void OnTakeDamageAnimation()
    {
        if (effectPrefab == null) return;
        // T?o instance c?a hi?u ?ng n? t?i v? trí bullet
        Transform pos = transform;
        pos.position = new Vector3(pos.position.x, pos.position.y - 1f, pos.position.z);
        ParticleSystem effectInstance = Instantiate(effectPrefab, pos.position, Quaternion.identity);
        effectInstance.Play(); // Phát hi?u ?ng

        ObjectPool.Instance.ReturnToPool("Skill", gameObject);

        // (Tùy ch?n) H?y hi?u ?ng sau khi hoàn t?t
        Destroy(effectInstance.gameObject, effectInstance.main.duration); // H?y khi hi?u ?ng k?t thúc
    }
}
