using UnityEngine;
using System.Collections;

public class WarningZone : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject warningUI;
    [SerializeField] private float warningTime = 5f;

    [Header("Player Settings")]
    [SerializeField] private Transform player;        // Spieler-Transform (im Inspector zuweisen)
    [SerializeField] private Transform startPosition; // Startposition (Empty GameObject im Inspector)

    private Coroutine warningRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (warningRoutine == null)
            {
                warningRoutine = StartCoroutine(WarningCountdown());
            }
            warningUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (warningRoutine != null)
            {
                StopCoroutine(warningRoutine);
                warningRoutine = null;
            }
            warningUI.SetActive(false);
        }
    }

    private IEnumerator WarningCountdown()
    {
        float timer = warningTime;

        while (timer > 0f)
        {
            Debug.Log("Warning Timer: " + timer);

            yield return new WaitForSeconds(1f);
            timer--;
        }

        // Timer abgelaufen -> Spieler zurücksetzen
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
