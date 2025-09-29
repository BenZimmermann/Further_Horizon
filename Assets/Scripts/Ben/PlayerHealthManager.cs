using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    // Maximum health of the player
    public int maxHealth = 10;
    public int health;
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);
    [SerializeField] private WindowType windowType = WindowType.Console;

    [Header("UI Settings")]
    // List of heart/health images in UI (drag & drop in Inspector)
    [SerializeField] private List<Image> healthImages = new List<Image>();
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
    }

    private void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
    }
    /// <summary>
    /// Sets the health to a new value and updates UI.
    /// If health reaches 0 -> triggers death logic.
    /// </summary>
    public void SetHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        // Update the UI images to reflect health
        UpdateHealthUI();

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
            PauseMenuController.Instance.OpenWindow(windowType);
        }
    }

    /// <summary>
    /// Reduces health by damage amount.
    /// </summary>
    public void TakeDamage(int damage)
    {
        SetHealth(health - damage);
        //Debug.Log("Player took " + damage + " damage. Current health: " + health);
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Count; i++)
        {
            // Enable images up to current health, disable the rest
            healthImages[i].enabled = (i < health);
        }
    }

    /// <summary>
    /// Heals the player by a given amount.
    /// </summary>
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
#region alt--
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
#endregion