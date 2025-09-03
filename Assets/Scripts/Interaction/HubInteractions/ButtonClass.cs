using UnityEngine;

public class ButtonClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Activate";
    public Material highlightMaterial;

    [Header("Trigger Events")]
    [SerializeField] GameObject obj;

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private bool used = false;

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
        used = true;
        isEnabled = false;
        Destroy(obj);
        Remove();
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