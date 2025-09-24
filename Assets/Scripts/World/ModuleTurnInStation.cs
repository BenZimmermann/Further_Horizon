using UnityEngine;

public class ModuleTurnInStation : MonoBehaviour
{
    [Header("Welches Modul muss abgegeben werden?")]
    [SerializeField] private ItemDefinition requiredModuleItem; // z.B. Heatexchanger

    [Header("Optionales Feedback")]
    [SerializeField] private bool logFeedback = true;

    private InventoryManager inventory;

    private void Awake()
    {
        inventory = InventoryManager.Instance;
    }

    // Per UI-Button verbinden ODER per Interact-Flow aufrufen
    public void TryTurnIn()
    {
        if (inventory == null || requiredModuleItem == null)
        {
            Debug.LogWarning("[TurnIn] Inventory/Module-Item fehlt.");
            return;
        }

        // Muss im Inventar vorhanden sein
        if (!inventory.HasEnoughItems(requiredModuleItem, 1))
        {
            if (logFeedback) Debug.Log($"[TurnIn] {requiredModuleItem.displayName} nicht vorhanden.");
            return;
        }

        // 1) Aus Inventar entfernen
        bool ok = inventory.RemoveItem(requiredModuleItem, 1);
        if (!ok)
        {
            if (logFeedback) Debug.LogWarning("[TurnIn] Entfernen aus Inventar fehlgeschlagen.");
            return;
        }

        // 2) In GameData als pending vormerken
        var dpm = DataPersistenceManager.Instance;
        var data = dpm?.GetGameData();
        if (data == null)
        {
            Debug.LogError("[TurnIn] Kein GameData vorhanden.");
            return;
        }

        string id = requiredModuleItem.itemId;
        if (!data.pendingInstallModuleIds.Contains(id))
            data.pendingInstallModuleIds.Add(id);

        if (logFeedback) Debug.Log($"[TurnIn] Modul vorgemerkt: {requiredModuleItem.displayName} (id:{id})");

        // 3) Speichern (sicher ist sicher)
        dpm.SaveGame();
    }
}
