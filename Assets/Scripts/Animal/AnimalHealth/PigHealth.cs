using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 15;

    private int currentHealth;

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
