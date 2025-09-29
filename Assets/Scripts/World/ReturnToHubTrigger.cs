using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHubTrigger : MonoBehaviour
{
    [Header("Hub Scene")]
    [SerializeField] private string hubSceneName = "Hub"; // anpassen, falls abweichend

    [Header("Quest-/Modulitems")]
    [SerializeField] private ItemDefinition heatexchanger;
    [SerializeField] private ItemDefinition fluiedtank;
    [SerializeField] private ItemDefinition fusionskonduktor;


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
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        string currentScene = SceneManager.GetActiveScene().name;

        bool allowed = false;
        var inv = InventoryManager.Instance;

        if (inv == null)
        {
            Debug.LogError("[ReturnToHub] Kein InventoryManager gefunden!");
            return;
        }

        switch (currentScene)
        {
            case "Cryovista":
                allowed = inv.HasItem(heatexchanger);
                break;
            case "Umbra":
                allowed = inv.HasItem(fluiedtank);
                break;
            case "Singara":
                allowed = inv.HasItem(fusionskonduktor);
                break;
        }

        if (allowed)
        {
            Debug.Log("[ReturnToHub] Bedingung erfüllt, speichere und wechsle zum Hub.");

            // Sicherstellen, dass der Manager existiert
            var dpm = DataPersistenceManager.Instance;
            if (dpm != null)
            {
                dpm.SaveGame();
            }
            else
            {
                Debug.LogError("[ReturnToHub] DataPersistanceManager nicht gefunden!");
            }

            SceneManager.LoadScene(hubSceneName);
        }
        else
        {
            Debug.Log("[ReturnToHub] Bedingung NICHT erfüllt - Rückkehr blockiert.");
            // optional: UI Hinweis anzeigen
        }
    }
}
