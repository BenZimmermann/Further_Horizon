using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] public string spawnId;  // z.B. "Cryo_Start", "Cryo_Mine_A"
    [SerializeField] public bool isDefault;  // genau einer pro Szene

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(spawnId))
            spawnId = name; // Fallback: GameObject-Name
    }
}
