// PlayerHealthManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth = 50;

    // Singleton Pattern - Instance für globalen Zugriff
    public static PlayerHealthManager Instance { get; private set; }
    public int health;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Optional: DontDestroyOnLoad(gameObject); // Falls das Objekt zwischen Szenen bestehen bleiben soll
    }

    void Update()
    {
        Debug.Log("Player Health: " + health);
    }

    private void Start()
    {
        health = maxHealth;
        MaxHealth();
        SetHealth(maxHealth);
    }

    public void MaxHealth()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        healthSlider.value = health;

        // Sicherstellen, dass Health nicht unter 0 fällt
        if (health <= 0)
        {
            health = 0;
            // Hier könnte Game Over Logic hinzugefügt werden
            Debug.Log("Player is dead!");
        }
    }

    // Neue Methode für Schadenszufügung
    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth); // Begrenzt Health zwischen 0 und maxHealth
        SetHealth(health);
        Debug.Log("Player took " + damage + " damage. Current health: " + health);
    }
    public void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}