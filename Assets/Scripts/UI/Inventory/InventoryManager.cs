using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemListPanel;         // Parent, z. B. dein ItemList_TMP ersetzen
    [SerializeField] private GameObject itemButtonPrefab;     // Dein ItemButtonInventory Prefab

    // Daten
    private Dictionary<ItemDefinition, int> items = new();                    // Item  Anzahl
    private Dictionary<ItemDefinition, InventoryButtonUI> itemButtons = new(); // Item  UI-Button

    /// <summary>
    /// Fügt ein Item ins Inventar ein (oder erhöht die Anzahl).
    /// </summary>

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
    public void AddItem(ItemDefinition itemData, int amount = 1)
    {
        Debug.LogWarning($"AddItem aufgerufen mit: {itemData.name}, Amount: {amount}");

        // Prüfen, ob Item schon existiert
        if (items.ContainsKey(itemData))
        {
            items[itemData] += amount;

            // Button-Text updaten
            itemButtons[itemData].UpdateCount(items[itemData]);
        }
        else
        {
            items[itemData] = amount;

            // Button erzeugen
            GameObject buttonObj = Instantiate(itemButtonPrefab, itemListPanel);
            InventoryButtonUI buttonUI = buttonObj.GetComponent<InventoryButtonUI>();

            buttonUI.Setup(itemData, amount);

            // Klick-Event hinzufügen
            //buttonUI.Button.onClick.AddListener(() => OnItemClicked(buttonUI));

            // In Dictionary merken
            itemButtons[itemData] = buttonUI;
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn ein Item-Button angeklickt wird.
    /// </summary>
    private void OnItemClicked(InventoryButtonUI buttonUI)
    {
        ItemDefinition data = buttonUI.GetItemData();

        Debug.Log($"Item geklickt: {data.displayName}");
        // TODO: Hier ItemModel_TMP und ItemDescription_TMP befüllen
    }
}
