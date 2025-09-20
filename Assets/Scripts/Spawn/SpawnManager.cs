using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Transform playerRoot; // Player / Character rig
    private string chosenSpawnId;                  // aus Save geladen oder Default

    public void LoadData(GameData data)
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // NEU: über Helper-Funktion aus GameData holen
        chosenSpawnId = data.GetSpawnForScene(scene);

        var points = FindObjectsOfType<SpawnPoint>(true);
        SpawnPoint target = null;

        if (!string.IsNullOrEmpty(chosenSpawnId))
            target = points.FirstOrDefault(p => p.spawnId == chosenSpawnId);

        if (target == null)
            target = points.FirstOrDefault(p => p.isDefault) ?? points.FirstOrDefault();

        if (target != null && playerRoot != null)
        {
            playerRoot.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        }
    }


    public void SaveData(GameData data)
    {
        // nichts zu tun – wir schreiben bei Checkpoint
    }
}
