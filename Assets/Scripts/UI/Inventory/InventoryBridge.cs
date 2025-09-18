using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Brücke zwischen ItemDefinition-Assets, gespeicherten ItemSaveData und der UI.
/// </summary>
public class InventoryBridge : MonoBehaviour, IDataPersistence
{
    [Header("Referenzen")]
    [SerializeField] private Transform itemListParent; // Container für Items in der UI
    [SerializeField] private GameObject itemUIPrefab;  // UI-Element-Vorlage für ein Item
    [SerializeField] private ItemDefinition[] allItems; // Alle verfügbaren Items

    private GameData gameData;


    public void LoadData(GameData data)
    {
        gameData = data;
        RefreshUI();
    }

    public void SaveData(GameData data)
    {
        // Beispiel: Wenn man in der UI Items verändert, würde man sie hier zurückschreiben.
        // In diesem Grundgerüst gehen wir davon aus, dass Items nur gesammelt werden.
    }

    private void RefreshUI()
    {
        // Bestehende UI-Einträge löschen
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        // Neue UI-Einträge erstellen
        foreach (var itemDef in allItems)
        {
            // Gesammelte Anzahl aus GameData suchen
            var itemSave = gameData.items.FirstOrDefault(i => i.itemId == itemDef.itemId);
            int collected = itemSave != null ? itemSave.collected : 0;

            // UI-Element erstellen
            GameObject uiItem = Instantiate(itemUIPrefab, itemListParent);

            // TMPs & Icon setzen (Prefab muss vorbereitet sein)
            uiItem.transform.Find("Icon").GetComponent<Image>().sprite = itemDef.itemModelSprite;
            uiItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemDef.displayName;
            uiItem.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = itemDef.description;
            uiItem.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = $"{collected}/{itemDef.maxStack}";
        }
    }
}
