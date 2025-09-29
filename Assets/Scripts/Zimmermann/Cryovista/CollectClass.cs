
using UnityEngine;

//erbt von Interactable, um Interaktionen zu ermöglichen
public class CollectClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Collect";
    public Material highlightMaterial;

    private bool used = false;
    private Material[] originalMaterials;
    private Renderer objectRenderer;

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
    }
    public void Update() { }
    public void Interact()
    {
        if (used) return;
        if (!isEnabled) return;
        used = true; // Assuming 'used' is a field in this class to track interaction state
        isEnabled = false;
        Remove();
        Destroy(gameObject);
    }
    public void Apply()
    {
        {
            if (used || highlightMaterial == null) return;

            Material[] newMats = new Material[originalMaterials.Length + 1];
            originalMaterials.CopyTo(newMats, 0);
            newMats[newMats.Length - 1] = highlightMaterial;

            objectRenderer.materials = newMats;
        }
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
        return !used;
    }
}
