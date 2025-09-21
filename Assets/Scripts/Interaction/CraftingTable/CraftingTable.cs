using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingTable : MonoBehaviour
{
    [Header("Rezept")]
    [SerializeField] private CraftingRecipe recipe;

    [Header("UI")]
    [SerializeField] private TMP_Text collectableLabel; // Text auf "CollectableItemButton"
    [SerializeField] private TMP_Text questLabel;       // Text auf "QuestItemButton"
    [SerializeField] private Button craftButton;      // "CraftItemButton"

    private InventoryManager inventory;

    private void Awake()
    {
        inventory = InventoryManager.Instance;
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    // --- wird vom Craft-Button aufgerufen ---
    public void OnCraftPressed()
    {
        if (recipe == null || inventory == null) return;

        // 1) prüfen
        foreach (var ing in recipe.ingredients)
        {
            if (!inventory.HasEnoughItems(ing.item, ing.amount))
            {
                // Optional: kurze Info auf dem Button-Label
                // (ohne zusätzlichen TMP im Layout)
                if (collectableLabel != null)
                    collectableLabel.text = $"{ing.item.displayName} fehlt!";
                return;
            }
        }

        // 2) abziehen
        foreach (var ing in recipe.ingredients)
        {
            if (!inventory.RemoveItem(ing.item, ing.amount))
            {
                // falls Zwischenstand nicht passt, abbrechen
                RefreshUI();
                return;
            }
        }

        // 3) Ergebnis hinzufügen
        inventory.AddItem(recipe.outputItem, recipe.outputAmount);

        // 4) UI aktualisieren
        RefreshUI();
    }

    // --- zeigt Name + Count der 2 Zutaten und setzt Interactable ---
    private void RefreshUI()
    {
        if (inventory == null || recipe == null || recipe.ingredients == null || recipe.ingredients.Length == 0)
        {
            if (craftButton) craftButton.interactable = false;
            return;
        }

        // genau 2 zutaten
        int ok = 0;
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            var ing = recipe.ingredients[i];
            int have = inventory.GetCount(ing.item);

            if (i == 0 && collectableLabel != null)
                collectableLabel.text = $"{ing.item.displayName}  {have}/{ing.amount}";

            if (i == 1 && questLabel != null)
                questLabel.text = $"{ing.item.displayName}  {have}/{ing.amount}";

            if (have >= ing.amount) ok++;
        }

        if (craftButton != null)
            craftButton.interactable = (ok == recipe.ingredients.Length);
    }
}
