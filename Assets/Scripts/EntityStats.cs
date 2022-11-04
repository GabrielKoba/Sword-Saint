using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UIElements;

public class EntityStats : MonoBehaviour, IDamageable {

    [Header("Health Stats")]
    public bool isAlive;
    public int currentHealth;
    public int maxHealth = 100;

    [Header("Blocking Stats")]
    public bool isBlocking;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        if (currentHealth > 0) {
            currentHealth -= damage;

            if (currentHealth <= 0) {
                isAlive = false;
            }
        }
    }
}