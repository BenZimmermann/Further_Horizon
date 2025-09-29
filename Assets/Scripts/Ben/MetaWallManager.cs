using UnityEngine;
using System.Collections;

public class WarningZone : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject warningUI;      
    [SerializeField] private float warningTime = 5f;    //the time the player has until the gets teleported back

    [Header("Player Settings")]
    [SerializeField] private Transform player;         //position of the player
    [SerializeField] private Transform startPosition;  //position of the spawnObject

    private Coroutine warningRoutine;

    private void OnTriggerEnter(Collider other)
    {
        //using a onTriggerEnter to detect the player
        if (other.CompareTag("Player"))
        {
            if (warningRoutine == null)
            {
                warningRoutine = StartCoroutine(WarningCountdown());
            }
            //set the warning canvas active to notify the player
            warningUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (warningRoutine != null)
            {
                //if the player exists the area bevore the time runs out the timer will reset
                StopCoroutine(warningRoutine);
                warningRoutine = null;
            }
            warningUI.SetActive(false);
        }
    }

    /// <summary>
    /// counting 5s up and teleport the player back to spawn
    /// </summary>
    private IEnumerator WarningCountdown()
    {
        float timer = warningTime;

        while (timer > 0f)
        {
            Debug.Log("Warning Timer: " + timer);
            
            yield return new WaitForSeconds(1f);
            timer--;
        }

        // if the time runs out, teleport the player back
        OnWarningTimeout();
        warningRoutine = null;
    }

    private void OnWarningTimeout()
    {
        Debug.Log("Timer abgelaufen! Spieler wird zurückgesetzt.");

        if (player != null && startPosition != null)
        {
            player.transform.position = startPosition.position;
            player.transform.rotation = startPosition.rotation;
        }

        warningUI.SetActive(false);
    }
}
