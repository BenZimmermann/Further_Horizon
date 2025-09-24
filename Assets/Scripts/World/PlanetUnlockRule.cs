using UnityEngine;

[CreateAssetMenu(fileName = "PlanetUnlockRule", menuName = "Progression/Planet Unlock Rule")]
public class PlanetUnlockRule : ScriptableObject
{
    public string planetSceneName;                 // z.B. "Singara"
    public ItemDefinition[] requiredModules;       // z.B. [Heatexchanger und Fluiedtank]
}
