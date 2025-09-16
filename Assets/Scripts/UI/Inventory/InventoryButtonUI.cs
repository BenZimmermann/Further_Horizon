using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText; // Referenz zum TMP im Prefab
    private ItemDefinition itemData;                  // Das ScriptableObject-Item
    private int itemCount;                            // Anzahl des Items

    public Button Button { get; private set; }        // Damit InventoryManager auf den Button zugreifen kann

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    /// <summary>
    /// Initialisiert den Button mit Item-Daten.
    /// </summary>
    public void Setup(ItemDefinition data, int count)
    {
        itemData = data;
        itemCount = count;
        RefreshUI();
    }

    /// <summary>
    /// Wenn Anzahl sich ‰ndert, nur Text updaten.
    /// </summary>
    public void UpdateCount(int newCount)
    {
        itemCount = newCount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        itemText.text = $"{itemData.displayName} x{itemCount}";
    }

    /// <summary>
    /// Getter, damit InventoryManager weiﬂ, welches Item das ist.
    /// </summary>
    public ItemDefinition GetItemData() => itemData;
}
