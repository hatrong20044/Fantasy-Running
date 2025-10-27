using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] int health = 5;
    [SerializeField] private HealthBar HealthBar;
    [SerializeField] private float maxHealth = 5;

    public void OnTakeDamage(int damage)
    {
        this.health -= damage;
        
        HealthBar.SetProgress((health/maxHealth), 1.5f);

        if(health == 0)
        {
            OnDied();
        }
    }

    public bool SpawnSkillAble()
    {
        if(health > 1) return true;
        return false;
    }

    public void OnDied()
    {
        // Gọi sự kiện OnBossFinished với đúng kiểu BossBase
        if (BossManager.instance != null)
        {
            BossBase bossBase = GetComponent<BossBase>();
            if (bossBase != null)
            {
                BossManager.instance.OnBossFinished(bossBase);
            }
            else
            {
                Debug.LogError("BossBase component not found on " + gameObject.name);
            }
        }
        else
        {
            Debug.LogError("BossManager instance is null!");
        }

        SkillSpawner.Instance.resetItemUsed();

        // Hủy GameObject của boss
        Destroy(gameObject);
    }
}
