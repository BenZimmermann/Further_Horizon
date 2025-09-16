using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InventoryController : MonoBehaviour
{

    [Header("Window Settings")]
    [SerializeField] private WindowType windowType = WindowType.Inventar;
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);
    private bool isOpen = false;
    private PlayerInput playerInput;

    //[SerializeField] private GameObject inventoryPanel;
    //[SerializeField] private InventoryInputHandler inputHandler;

    //private bool isVisible = false;

    //private void Start()
    //{
    //    if (inventoryPanel != null)
    //        inventoryPanel.SetActive(false);

    //    if (inputHandler != null)
    //        inputHandler.OnInventoryToggle += ToggleInventory; //  hört auf das Event
    //}

    //private void OnDestroy()
    //{
    //    if (inputHandler != null)
    //        inputHandler.OnInventoryToggle -= ToggleInventory; //  sauber unsubscriben
    //}

    //private void ToggleInventory()
    //{
    //    isVisible = !isVisible;
    //    if (inventoryPanel != null)
    //        inventoryPanel.SetActive(isVisible);
    //}
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Inventar"].performed += onInv;
    }


    private void onInv(CallbackContext ctx)
    {
        Debug.Log("Inv gedrückt");
        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            //PauseMenuController.Instance.CloseWindow(windowType);
            isOpen = false;
            return;
        }

        Debug.Log($"Console {windowType} wird geöffnet");
        PauseMenuController.Instance.OpenWindow(windowType);
        isOpen = true;
    }
}
