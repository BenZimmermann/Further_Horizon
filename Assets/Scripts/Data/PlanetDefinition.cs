using UnityEngine;

[CreateAssetMenu(fileName = "PlanetDefinition", menuName = "Game/PlanetDefinition")]
public class PlanetDefinition : ScriptableObject // Definiert einen Planeten im Spiel
{
    [Header("Anzeige")]
    public string displayName;
    [TextArea] public string description;
    public string temperature;

    [Header("Logik")]
    public string sceneName;
    public bool isLocked = false;

    [Header("Image")]
    public Sprite planetModelSprite;
}
