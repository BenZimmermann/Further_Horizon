using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, Interactable
{
    [Header("Item Reference")]
    [SerializeField] private ItemDefinition itemDefinition;
    [SerializeField] private int amount = 1;

    private bool collected = false;
    public void Awake() { }
    public void Update() { }

    public void Apply() { }
    public void Remove() { }

    public string GetInteractionText()
    {
        return collected ? "" : $"Sammle {itemDefinition.displayName}";
    }

    public bool IsInteractable()
    {
        return !collected;
    }

    public void Interact()
    {
        if (collected) return;
        collected = true; // setz auf eingesammelt -> Verbindung in der ItemDefinition.cs
        // TODO: Inventar + SaveGame
        Debug.LogWarning($"Picked up {itemDefinition.displayName}");// gucke ob der zählt
        // Item ins Inventar geben
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemDefinition, amount);
        }
        else
        {
            Debug.LogError("Kein InventoryManager gefunden!");
        }

        Destroy(gameObject); // Objekt aus der Welt entfernen

    }
}
