using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using static PauseMenuController;
public class GeneratorClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;
    private bool used = false;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Activate Generator";
    public Material highlightMaterial;

    [Header("Trigger Events")]
    [SerializeField] GameObject buttonFunc;
    public bool Active { get; private set; } = false;

    private Material[] originalMaterials;
    private Renderer objectRenderer;

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
    }

    public void Update()
    {

    }

    public void Interact()
    {
        if (used) return;
        if (!isEnabled) return;
        used = true; // Assuming 'used' is a field in this class to track interaction state
        isEnabled = false;
        Active = true;
        GeneratorController.Instance.RegisterGenerator();
        Remove();
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