using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Anzeige")]
    public string itemId;        // Eindeutige ID (z.B. "potion_01")
    public string displayName;   // Name in der UI
    [TextArea] public string description;
    public Sprite icon;          // Icon für UI

    [Header("Eigenschaften")]
    public int maxStack = 99;    // maximale Anzahl, die gesammelt werden kann
    public bool isQuestItem = false; // ob Item Pflicht ist (z. B. für Quest)


    // Später beliebig erweiterbar, z. B.:
    /*
    public int powerValue;
    public float weight;
    public Rarity rarity; // (Enum: Common, Rare, Legendary)
    */
}
