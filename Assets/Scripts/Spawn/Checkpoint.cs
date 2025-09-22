// Assets/Scripts/Spawn/Checkpoint.cs
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string spawnId;   // z.B. "Cryo_Start", "Cryo_Mine_A"
    private string sceneName;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Passe das bei Bedarf an euer System an (Tag/Layer)
        if (!other.CompareTag("Player")) return;

        // *** ACHTUNG: richtige Klasse/Schreibweise ***
        var dpm = DataPersistenceManager.Instance;
        if (dpm == null) return;

        var data = dpm.GetGameData();
        if (data == null) return;

        // Dictionary absichern
        //if (data.lastSpawnByScene == null)
        //    data.lastSpawnByScene = new System.Collections.Generic.Dictionary<string, string>();

        data.SetSpawnForScene(sceneName, spawnId);
        dpm.SaveGame();

        Debug.Log($"[Checkpoint] Gespeichert: {sceneName} -> {spawnId}");
    }
}
