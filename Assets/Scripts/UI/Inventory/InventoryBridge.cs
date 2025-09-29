using System.Collections.Generic;
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

    // InventoryManager UI Info Referenzen
    [SerializeField] private Transform itemListPanel;     // Parent für die Buttons
    [SerializeField] private Image itemModel_Imageholder;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;
    [SerializeField] private TextMeshProUGUI planetNameTMP;
    [SerializeField] private TextMeshProUGUI planetInfoTMP;
    [SerializeField] private TextMeshProUGUI planetTemperatureTMP;
    [SerializeField] private Image planetModel_Imageholder;

    // >>> Öffentliche, schreibgeschützte Zugriffspunkte (werden von InventoryManager benutzt)
    public Transform ItemListParent => itemListParent; // Container für Items in der UI
    public GameObject ItemButtonPrefab => itemUIPrefab; // UI-Element-Vorlage für ein Item
    public List<ItemDefinition> AllItems => allItems != null ? allItems.ToList() : new List<ItemDefinition>(); // Alle verfügbaren Items in Liste

    private GameData gameData; // Referenz auf die aktuellen Spieldaten
    

    private void Awake()
    {
        // An InventoryManager melden
        InventoryManager.Instance?.SetBridge(this, itemListPanel, itemModel_Imageholder, planetModel_Imageholder, itemDescriptionTMP, planetInfoTMP, planetNameTMP, planetTemperatureTMP);
    }
    public void LoadData(GameData data)
    {
        gameData = data;
        RefreshUI();
    }

    public void SaveData(GameData data)
    {
        
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
            var itemSave = gameData.inventory.FirstOrDefault(i => i.itemId == itemDef.itemId);
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
