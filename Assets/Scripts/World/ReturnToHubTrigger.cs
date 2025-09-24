using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHubTrigger : MonoBehaviour
{
    [SerializeField] private string hubSceneName = "Hub"; // anpassen, falls abweichend

    // Du kannst das via UI-Button aufrufen ODER an deinen bestehenden Interact-Flow hängen.
    public void ReturnToHubNow()
    {
        // (optional) merken, woher wir kommen
        var dpm = DataPersistenceManager.Instance;
        var data = dpm?.GetGameData();
        if (data != null)
        {
            data.lastReturnFromScene = SceneManager.GetActiveScene().name;
            dpm.SaveGame();
        }

        SceneManager.LoadScene(hubSceneName);
    }

    // Falls du lieber per Trigger + Taste F arbeiten willst, kannst du hier andocken:
    private void OnTriggerStay(Collider other)
    {
        // Minimalbeispiel: F drücken
        if (other.CompareTag("Player"))
        {
            ReturnToHubNow();
        }
    }
}
