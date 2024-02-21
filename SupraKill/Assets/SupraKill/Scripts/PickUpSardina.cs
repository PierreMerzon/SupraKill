using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSardina : MonoBehaviour
{
    PlayerController playerHealth;
    public float healthBonus = 15f;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerHealth.currentHealth < playerHealth.maxHealth)
        {
            Destroy(gameObject);
            playerHealth.currentHealth = playerHealth.currentHealth + healthBonus;
        }
    }
}
