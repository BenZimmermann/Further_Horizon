using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText; // Referenz zum TMP im Prefab
    private ItemDefinition itemData;                  // Das ScriptableObject-Item
    private int itemCount;                            // Anzahl des Items

    public Button Button { get; private set; }        // Damit InventoryManager auf den Button zugreifen kann


    // Initialisiert den Button mit Item-Daten.
    public void Setup(ItemDefinition data, int count)
    {
        itemData = data;          // Setze Item-Daten
        itemCount = count;       // Setze Anzahl

        Button = GetComponent<Button>();    // Hole Button-Komponente
        if (Button == null)                 // Fehlerprüfung
            Debug.LogError("[InventoryButtonUI] Button-Komponente am Prefab-Root fehlt!");

        RefreshUI();               // Aktualisiere UI
    }

    // Wenn Anzahl sich ändert, nur Text updaten.
    public void UpdateCount(int newCount)
    {
        itemCount = newCount; // Setze neue Anzahl
        RefreshUI();
    }

    private void RefreshUI() // Aktualisiert UI
    {
        if (itemText == null)
        {
            Debug.LogError("[InventoryButtonUI] itemText (TMP) ist NICHT zugewiesen! Prefab: " + name);
            return; // Zweck: NullReference verhindern und Ursache sichtbar machen
        }
        if (itemData == null)
        {
            Debug.LogError("[InventoryButtonUI] itemData ist null – Setup wurde nicht korrekt aufgerufen?");
            return; // Zweck: NullReference verhindern und Ursache sichtbar machen
        }
        itemText.text = $"{itemData.displayName} x{itemCount}"; // z.B. "Luminessence x3"
    }

    // Getter, damit InventoryManager weiß, welches Item das ist.
    public ItemDefinition GetItemData() => itemData;
}
