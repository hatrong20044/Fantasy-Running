using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] int health = 5;
    [SerializeField] private HealthBar HealthBar;
    [SerializeField] private int maxHealth = 5;

    public void OnTakeDamage(int damage)
    {
        this.health -= damage;
        if (!transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(true); // Kích hoạt lại Canvas
        }
        gameObject.SetActive(true);
        HealthBar.SetProgress(health/maxHealth, 3);
    }

    public void OnDied()
    {
        
    }
}
