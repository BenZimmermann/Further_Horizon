using UnityEngine;

[CreateAssetMenu(fileName = "PlanetDefinition", menuName = "Game/PlanetDefinition")]
public class PlanetDefinition : ScriptableObject
{
    [Header("Anzeige")]
    public string displayName;
    [TextArea] public string description;

    [Header("Logik")]
    public string sceneName;
    public bool isLocked = false;
}
