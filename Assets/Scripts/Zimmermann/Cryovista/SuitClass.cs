using UnityEngine;

//erbt von Interactable, um Interaktionen zu ermöglichen
public class SuitClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;
    private bool used = false;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Equip";
    public Material highlightMaterial;

    [Header("Trigger Events")]
    public GameObject suit;                 //suit Object
    private Material[] originalMaterials;
    private Renderer objectRenderer;
    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
    }
    public void Update() {}
    public void Interact()
    {
        if (used) return;
        if (!isEnabled) return;
        used = true; //'used' is a field in this class to track interaction state
        isEnabled = false;
        //signal to the EnvioremtDamageController that the player is now warm => stop envioremt damage
        EnviormentDamageController.Instance.IsWarm = true;
        Remove();
        Destroy(suit);
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