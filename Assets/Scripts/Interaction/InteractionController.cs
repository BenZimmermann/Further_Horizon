using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    [Header("UI Elements")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Reverences")]
    [SerializeField] private Camera playerCamera;

    private PlayerInput playerInput;
    private Interactable currentInteractable;

    [Header("Debug")]
    private RaycastHit _hitInfo;
    private bool _didHit;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Interact"].performed += onInteract;

        if (playerCamera == null)
            playerCamera = Camera.main;

        uiPanel.SetActive(false);
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward * 0.3f);
        _didHit = Physics.Raycast(ray, out _hitInfo, interactDistance);//debug purposes
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactLayer.Contains(hit.collider.gameObject.layer))
            {
                if (interactable != null && interactable.IsInteractable())
                {
                    if (currentInteractable != interactable)
                    {
                        SetInteractable(interactable);
                    }
                    return;
                }
            }
        }

        ClearInteractable();
    }

    private void SetInteractable(Interactable interactable)
    {
        currentInteractable?.Remove();
        currentInteractable = interactable;
        currentInteractable.Apply();
        interactionText.text = currentInteractable.GetInteractionText();
        uiPanel.SetActive(true);
    }

    private void ClearInteractable()
    {
        currentInteractable?.Remove();
        currentInteractable = null;
        uiPanel.SetActive(false);
    }

    private void onInteract(CallbackContext ctx)
    {
        if (currentInteractable == null) return;

        currentInteractable.Interact();
        currentInteractable.Remove();
        currentInteractable = null;
        uiPanel.SetActive(false); 
    }
    #region Gizmos
    private void OnDrawGizmos()
    {
        if (playerCamera == null) return;

        Gizmos.color = _didHit ? Color.green : Color.red;
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance);
    }
    #endregion
}
public static class UnityExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}