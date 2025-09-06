using UnityEngine;
using System.Collections;

public class EnviormentDamageController : MonoBehaviour
{
    public static EnviormentDamageController Instance { get; private set; }
    public bool IsWarm = false;
    public int EnviormentDamage = 5;
    public float DamageTick = 5f;

    private bool isDamaging = false; // Verhindert mehrfaches Starten der Coroutine

    private void Awake()
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
    void Update()
    {
        if (!IsWarm && !isDamaging)
        {
            Debug.Log("Environment Damage Active");
            StartCoroutine(SnowStormDamage());
        }
    }

    private IEnumerator SnowStormDamage()
    {
        isDamaging = true;

        while (!IsWarm) // Schleife läuft solange es nicht warm ist
        {
            yield return new WaitForSeconds(DamageTick);

            // Überprüfen ob PlayerHealthManager existiert
            if (PlayerHealthManager.Instance != null)
            {
                Debug.Log("Applying Environment Damage: " + EnviormentDamage);
                PlayerHealthManager.Instance.TakeDamage(EnviormentDamage);
            }
            else
            {
                Debug.LogError("PlayerHealthManager Instance not found!");
                break;
            }
        }

        isDamaging = false;
        Debug.Log("Environment damage stopped - player is warm");
    }

    // Optional: Methode um Wärme-Status zu ändern
    public void SetWarmStatus(bool warm)
    {
        IsWarm = warm;
        if (warm)
        {
            Debug.Log("Player is now warm - stopping environment damage");
        }
    }
}