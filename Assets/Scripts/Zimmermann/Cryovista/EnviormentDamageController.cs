using UnityEngine;
using System.Collections;

public class EnviormentDamageController : MonoBehaviour
{
    public static EnviormentDamageController Instance { get; private set; }

    [Header("damage Rate")]
    public bool IsWarm = false;
    public int EnviormentDamage = 5;
    public float DamageTick = 1f;

    private bool isDamaging = false;

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
        //if the player is not warm he gets damage
        if (!IsWarm && !isDamaging)
        {
            Debug.Log("Environment Damage Active");
            StartCoroutine(SnowStormDamage());
        }
    }

    private IEnumerator SnowStormDamage()
    {
        isDamaging = true;

        while (!IsWarm) // run until the player is warm
        {
            yield return new WaitForSeconds(DamageTick);

            // check if the playerHealthManager exists
            if (PlayerHealthManager.Instance != null)
            {
                //Debug.Log("Applying Environment Damage: " + EnviormentDamage);
                //take the amount of damage provided as EnviormentDamage
                PlayerHealthManager.Instance.TakeDamage(EnviormentDamage);
            }
            else
            {
                Debug.LogError("PlayerHealthManager Instance not found!");
                break;
            }
        }
        //if bool is false => the player is warm
        isDamaging = false;
        Debug.Log("Environment damage stopped - player is warm");
    }

    //if the player is warm delete the current camera shader (theorie)
    public void SetWarmStatus(bool warm)
    {
        IsWarm = warm;
        if (warm)
        {
            FreezeEffectManager.Instance.SetFullScreenPass(false);
            Debug.Log("Player is now warm - stopping environment damage");
        }
    }
}