using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public enum ItemType
{
    Collectable,  // zählbar (z.B. Luminesenz, Gravimetall)
    QuestItem,    // einmalig (z.B. Heatexchanger(kaputt), Fluiedtank(kaputt))
    ModulItem     // Spezial-Quest-Item (z.B. Heatexchanger, Fluiedtank, Fusionkonduktor)
}

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identität")]
    [Tooltip("Eindeutige, stabile ID (z.B. 'heatexchanger_broken'). " +
        "Wird für Save/Load und Vergleiche verwendet.")]
    public string itemId;

    [Header("Anzeige")]
    [Tooltip("Name, der im UI angezeigt wird.")]
    public string displayName;

    [TextArea]
    [Tooltip("Beschreibung für Tooltips/Detail-Panel.")]
    public string description;

    [Tooltip("Icon für UI.")]
    public Sprite itemModelSprite;

    [Header("Optionale Verknüpfung")]
    public PlanetDefinition associatedPlanet;

    [Header("Klassifikation")]
    [Tooltip("Bestimmt das Verhalten (einmalig vs. stapelbar).")]
    public ItemType itemType = ItemType.Collectable;

    [Tooltip("Ob dieses Item über Crafting hergestellt/benötigt wird. " +
        "Nur eine Information(noch ohne Logik).")]
    public bool isCraftable = false;

    [Header("Stacking (nur für Collectables)")]
    [Min(1)]
    [Tooltip("Maximaler Stapel für Collectables. " +
        "Für Quest/Modul wird automatisch auf 1 gesetzt.")]
    public int maxStack = 99;

    // --- Optional: hier kann später Seltenheit, Gewicht etc. ergänzt werden ---
    // public Rarity rarity;
    // public float weight;
    // public string craftingRecipeId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Konsistenz sichern:
        // - Quest/Modul sind immer einmalig -> maxStack = 1
        // - Collectables dürfen stapeln
        if (itemType == ItemType.QuestItem || itemType == ItemType.ModulItem)
        {
            if (maxStack != 1) maxStack = 1;
        }

        // itemId zur Sicherheit trimmen
        if (!string.IsNullOrEmpty(itemId))
            itemId = itemId.Trim();
    }
#endif
}
