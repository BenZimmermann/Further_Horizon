using Unity.VisualScripting.Antlr3.Runtime;
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
    [SerializeField] GameObject map; // Das Fenster, das geöffnet/geschlossen wird
    //[SerializeField] GameObject obj; // Das Objekt mit dem Animator

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private Animator animator;

    private bool isWindowOpen = false; // Status des Fensters (offen/geschlossen)

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator-Komponente nicht gefunden!");
        }
        if (map != null)
        {
            map.SetActive(false);
        }
    }
    public void Update()
    {
    }

    public void Interact()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            if (map != null)
            {
                map.SetActive(true);
                animator.SetBool("Open", isWindowOpen);
            }
        }
        else
        {
            isWindowOpen = false;
            if (map != null)
            {
                map.SetActive(false);
            }
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