using UnityEngine;

public class ConsoleClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Use";
    public Material highlightMaterial;

    [Header("Trigger Events")]
    [SerializeField] GameObject map;
    [SerializeField, Tooltip("the globe")] GameObject globe;

    [Header("Window Settings")]
    [SerializeField] private WindowType windowType = WindowType.Console;

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private Animator animator;
    private bool isOpen = false;
    // Lokaler Status wird durch den PauseMenuController verwaltet
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        animator = globe.GetComponentInChildren<Animator>();
        originalMaterials = objectRenderer.materials;
    }

    public void Update()
    {
        // Synchronisiere das GameObject mit dem Window-Status
        if (map != null && map.activeSelf != IsActive)
        {
            map.SetActive(IsActive);
        }
    }

    public void Interact()
    {
        bool wasOpened = PauseMenuController.Instance.ToggleWindow(windowType);
        
        // Optional: Zusätzliche Aktionen beim Öffnen/Schließen
        if (wasOpened)
        {
            animator.SetBool("Open", isOpen);
            //Debug.Log($"Console {windowType} geöffnet");
        }
        else
        {
            //Debug.Log($"Console {windowType} geschlossen");
        }
    }

    public void Apply()
    {
        if (highlightMaterial == null) return;

        Material[] newMats = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(newMats, 0);
        newMats[newMats.Length - 1] = highlightMaterial;

        objectRenderer.materials = newMats;
    }

    public void Remove()
    {
        if (objectRenderer.materials.Length == originalMaterials.Length + 1)
        {
            objectRenderer.materials = originalMaterials;
        }
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public bool IsInteractable()
    {
        return isEnabled;
    }
}