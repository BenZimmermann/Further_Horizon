using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryInputHandler inputHandler;

    private bool isVisible = false;

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (inputHandler != null)
            inputHandler.OnInventoryToggle += ToggleInventory; //  hört auf das Event
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
            inputHandler.OnInventoryToggle -= ToggleInventory; //  sauber unsubscriben
    }

    private void ToggleInventory()
    {
        isVisible = !isVisible;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(isVisible);
    }
}
