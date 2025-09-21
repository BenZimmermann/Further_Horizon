using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;   // <- für Image
using TMPro;            // <- für TextMeshPro

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    [Header("UI References")]
    [SerializeField] private Transform itemListPanel;     // Parent für die Buttons
    [SerializeField] private GameObject itemButtonPrefab; // Dein ItemButtonInventory Prefab

    [Header("Detail-Felder")]
    [SerializeField] private Image itemModel_Imageholder;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;

    [Header("Planet Detail-Felder")]
    [SerializeField] private TextMeshProUGUI planetNameTMP;
    [SerializeField] private TextMeshProUGUI planetInfoTMP;
    [SerializeField] private TextMeshProUGUI planetTemperatureTMP;
    [SerializeField] private Image planetModel_Imageholder;

    // Kommunikation, isch meine Brücke damits läuft
    [SerializeField] private InventoryBridge bridge;


    // Daten
    private Dictionary<ItemDefinition, int> items = new();                      // Item  Anzahl
    private Dictionary<ItemDefinition, InventoryButtonUI> itemButtons = new(); // Item  UI-Button

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    /// Liefert die bereits verwendete ItemDefinition-Instanz mit gleicher itemId,
    /// falls sie in items oder itemButtons schon bekannt ist. Sonst die übergebene Instanz.
    private ItemDefinition Canonical(ItemDefinition incoming)
    {
        if (incoming == null) return null;

        foreach (var k in items.Keys)
            if (k != null && k.itemId == incoming.itemId)
                return k;

        foreach (var k in itemButtons.Keys)
            if (k != null && k.itemId == incoming.itemId)
                return k;

        return incoming;
    }


    // Item ins Inventar und wenn nötig hochzählen
    public void AddItem(ItemDefinition itemData, int amount = 1)
    {
        Debug.LogWarning($"[INV] AddItem {itemData.displayName}  id:{itemData.itemId}  inst:{itemData.GetInstanceID()}");

        if (itemData == null)
        {
            Debug.LogError("AddItem wurde mit null aufgerufen!");
            return;
        }

        //Debug.Log($"[Inventory] AddItem: {itemData.displayName}, Amount: {amount}");

        switch (itemData.itemType)
        {
            case ItemType.Collectable:
                HandleCollectable(itemData, amount);
                break;

            case ItemType.QuestItem:
            case ItemType.ModulItem:
                HandleUniqueItem(itemData);
                break;

            default:
                Debug.LogWarning($"Unbekannter ItemType: {itemData.itemType}");
                break;
        }
    }
    // Bereich für die normalen Items
    private void HandleCollectable(ItemDefinition itemData, int amount)
    {
        // Auf die bereits bekannte Instanz mappen (gleiche itemId)
        var key = Canonical(itemData);

        if (items.TryGetValue(key, out var current))
        {
            var newCount = Mathf.Min(current + amount, key.maxStack);
            items[key] = newCount;

            // UI vorhanden? -> count updaten (sonst kein KeyNotFound mehr)
            if (itemButtons.TryGetValue(key, out var btn))
                btn.UpdateCount(newCount);
            else
                CreateItemButton(key, newCount); // Falls Button wegen Prefab früher abgebrochen ist
        }
        else
        {
            var startCount = Mathf.Min(amount, key.maxStack);
            items[key] = startCount;
            CreateItemButton(key, startCount);
        }
    }
    // Extra Bereich für Quest-/Modul-Items (einmalig)
    private void HandleUniqueItem(ItemDefinition itemData)
    {
        var key = Canonical(itemData);

        if (items.ContainsKey(key))
        {
            //Debug.Log($"Einmaliges Item {key.displayName} ist schon im Inventar – ignoriert.");
            return;
        }
        items[key] = 1;
        CreateItemButton(key, 1);
    }

    // Button erstellen
    //private void CreateItemButton(ItemDefinition itemData, int amount)
    //{
    //    GameObject buttonObj = Instantiate(itemButtonPrefab, itemListPanel);
    //    InventoryButtonUI buttonUI = buttonObj.GetComponent<InventoryButtonUI>();

    //    buttonUI.Setup(itemData, amount);

    //    // Klick-Event für Details
    //    buttonUI.Button.onClick.AddListener(() => OnItemClicked(buttonUI));

    //    itemButtons[itemData] = buttonUI;
    //}
    private void CreateItemButton(ItemDefinition itemData, int amount)
    {
        Debug.LogWarning($"[INV] CreateButton key inst:{itemData.GetInstanceID()} -> {itemData.displayName}");

        // 1) Vorbedingung prüfen (Inspector-Referenzen)
        if (itemButtonPrefab == null)
        {
            //Debug.LogError("[Inventory] itemButtonPrefab ist NICHT zugewiesen!");
            return; // Zweck: Crash verhindern & klare Fehlermeldung liefern
        }
        if (itemListPanel == null)
        {
            //Debug.LogError("[Inventory] itemListPanel ist NICHT zugewiesen!");
            return; // Zweck: s.o.
        }

        // 2) Instanziieren
        GameObject buttonObj = Instantiate(itemButtonPrefab, itemListPanel, false);
        if (buttonObj == null)
        {
            Debug.LogError("[Inventory] Instantiate hat null zurückgegeben (sehr ungewöhnlich).");
            return; // Zweck: Absicherung, falls Prefab kaputt
        }

        // 3) Erwartete Komponente prüfen
        InventoryButtonUI buttonUI = buttonObj.GetComponent<InventoryButtonUI>();
        if (buttonUI == null)
        {
            Debug.LogError("[Inventory] Prefab '" + buttonObj.name + "' hat KEIN InventoryButtonUI!");
            return; // Zweck: sagt dir sofort, dass das falsche/halbfertige Prefab verwendet wird
        }

        // 4) Setup geschützt aufrufen (falls im Prefab ein Feld fehlt, z.B. itemText)
        try
        {
            Debug.Log($"[Inventory] Erzeuge Button für {itemData.displayName} (Amount {amount})");
            buttonUI.Setup(itemData, amount);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Inventory] Exception in buttonUI.Setup(): " + e);
            return; // Zweck: du siehst sofort, wenn z.B. itemText im Prefab nicht verlinkt ist
        }

        // 5) Button-Objekt prüfen
        if (buttonUI.Button == null)
        {
            Debug.LogError("[Inventory] Im Prefab fehlt die Button-Komponente am Root!");
            // Wir brechen ab, damit nicht die nächste Zeile wieder crasht.
            return;
        }

        // 6) Listener anhängen
        buttonUI.Button.onClick.AddListener(() => OnItemClicked(buttonUI));

        // 7) UI-Cache hinterlegen
        if (!itemButtons.ContainsKey(itemData))
            itemButtons[itemData] = buttonUI;
        else
            Debug.LogWarning("[Inventory] itemButtons hatte bereits einen Eintrag für " + itemData.displayName);
    }

    //wenn button angeklickt wird
    private void OnItemClicked(InventoryButtonUI buttonUI)
    {
        ItemDefinition data = buttonUI.GetItemData();
        if (data == null) return;

        Debug.Log($"Item geklickt: {data.displayName}");

        // Item Details
        if (itemModel_Imageholder != null)
        {
            if (data.itemModelSprite != null)
            {
                itemModel_Imageholder.sprite = data.itemModelSprite;
                itemModel_Imageholder.enabled = true;
            }
            else
            {
                itemModel_Imageholder.sprite = null;
                itemModel_Imageholder.enabled = false;
            }
        }

        if (itemDescriptionTMP != null)
            itemDescriptionTMP.text = data.description;

        // Planetendetails prüfen
        if (data.associatedPlanet != null) // NEU: Feld in ItemDefinition
        {
            if (planetNameTMP != null)
                planetNameTMP.text = data.associatedPlanet.displayName;

            if (planetInfoTMP != null)
                planetInfoTMP.text = data.associatedPlanet.description;

            if( planetTemperatureTMP != null)
                planetTemperatureTMP.text = data.associatedPlanet.temperature;

            if (planetModel_Imageholder != null)
                planetModel_Imageholder.sprite = data.itemModelSprite;
        }
    }
    private void ClearUIAndState()
    {
        // UI-Buttons unter dem List-Parent leeren
        var parent = bridge.ItemListParent;         // bridge: dein InventoryBridge-Ref
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);

        // Laufzeit-Maps leeren
        items.Clear();          // Dictionary<ItemDefinition,int>
        itemButtons.Clear();    // Dictionary<ItemDefinition, InventoryButtonUI>
    }

    public void LoadData(GameData data)
    {
        ClearUIAndState();
        
        if (data == null || data.inventory == null) return;

        // Für jede gespeicherte Zeile passenden ItemDefinition finden und hinzufügen
        foreach (var row in data.inventory)
        {
            var def = bridge.AllItems.FirstOrDefault(d => d.itemId == row.itemId);
            if (def == null)
            {
                Debug.LogWarning($"[INV/Load] Unbekannte itemId '{row.itemId}' – übersprungen.");
                continue;
            }

            // deine bestehende Logik nutzen, damit UI & Zähler korrekt aufgebaut werden
            AddItem(def, row.collected);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.inventory == null)
            data.inventory = new List<ItemSaveData>();
        else
            data.inventory.Clear();

        // Laufzeit-Map -> List<ItemSaveData>
        foreach (var kvp in items) // kvp.Key: ItemDefinition, kvp.Value: count
        {
            data.inventory.Add(new ItemSaveData
            {
                itemId = kvp.Key.itemId,
                collected = kvp.Value
            });
        }
    }

}