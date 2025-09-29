using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;
public class CraftingClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Craft";
    public Material highlightMaterial;

    [Header("Window Settings")]
    //reverence to window manager
    [SerializeField] private WindowType windowType = WindowType.Crafting;

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private Animator animator;
    private bool isOpen = false;
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        originalMaterials = objectRenderer.materials;
    }

    public void Update()
    {
    }
    /// <summary>
    /// getting a bool in pauseMenuController to detect witch window is the current
    /// </summary>
    public void Interact()
    {
        //logic to open windows in pauseMenuController
        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            isOpen = false;
            return;
        }

        Debug.Log($"Console {windowType} wird geöffnet");
        PauseMenuController.Instance.OpenWindow(windowType);
        isOpen = true;
    }

    /// <summary>
    /// appling the outline material to the current object
    /// </summary>
    public void Apply()
    {
        if (highlightMaterial == null) return;

        Material[] newMats = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(newMats, 0);
        newMats[newMats.Length - 1] = highlightMaterial;

        objectRenderer.materials = newMats;
    }
    /// <summary>
    /// removing the current outline material 
    /// </summary>
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