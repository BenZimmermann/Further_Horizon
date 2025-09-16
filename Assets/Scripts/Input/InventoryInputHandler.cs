using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InventoryInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Input Action Reference")]
    [SerializeField] private string inventory = "Inventar";

    private InputAction inventoryAction;

    // Event, das ausgelöst wird wenn E gedrückt wird
    public event Action OnInventoryToggle;

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);
        inventoryAction = mapReference.FindAction(inventory);

        // performed = Taste gedrückt
        inventoryAction.performed += _ => OnInventoryToggle?.Invoke();
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}