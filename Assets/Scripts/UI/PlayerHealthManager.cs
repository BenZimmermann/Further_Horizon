// PlayerHealthManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    //public Slider healthSlider;
    public int maxHealth = 10;
    public int health;
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);
    [SerializeField] private WindowType windowType = WindowType.Console;

    [Header("UI Settings")]
    [SerializeField] private List<Image> healthImages = new List<Image>(); // hier die 10 Images im Inspector reinziehen
    // Singleton Pattern - Instance für globalen Zugriff
    public static PlayerHealthManager Instance { get; private set; }

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

    private void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

    public void SetHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI();

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
            PauseMenuController.Instance.OpenWindow(windowType);
        }
    }

    public void TakeDamage(int damage)
    {
        SetHealth(health - damage);
        //Debug.Log("Player took " + damage + " damage. Current health: " + health);
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].enabled = (i < health);
            // Alle Images mit Index >= current health werden deaktiviert
        }
    }

    public void Heal(int amount)
    {
        SetHealth(health + amount);
    }
    public void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;

//public class PlayerHealthManager : MonoBehaviour
//{
//    [Header("Health Settings")]
//    public int maxHealth = 10; // jetzt 10 Herzen statt 50 HP
//    public int health;

//    [Header("UI Settings")]
//    [SerializeField] private List<Image> healthImages = new List<Image>(); // hier die 10 Images im Inspector reinziehen

//    // Singleton
//    public static PlayerHealthManager Instance { get; private set; }

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        health = maxHealth;
//        UpdateHealthUI();
//    }

//    public void SetHealth(int newHealth)
//    {
//        health = Mathf.Clamp(newHealth, 0, maxHealth);
//        UpdateHealthUI();

//        if (health <= 0)
//        {
//            Debug.Log("Player is dead!");
//            // Game Over Logik
//        }
//    }

//    public void TakeDamage(int damage)
//    {
//        SetHealth(health - damage);
//        Debug.Log("Player took " + damage + " damage. Current health: " + health);
//    }

//    private void UpdateHealthUI()
//    {
//        for (int i = 0; i < healthImages.Count; i++)
//        {
//            healthImages[i].enabled = (i < health);
//            // Alle Images mit Index >= current health werden deaktiviert
//        }
//    }

//    public void Heal(int amount)
//    {
//        SetHealth(health + amount);
//    }

//    private void OnDestroy()
//    {
//        if (Instance == this)
//        {
//            Instance = null;
//        }
//    }
//}
