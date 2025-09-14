using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;
public class PictureGameClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;
    private bool used = false;
    public static PictureGameClass ActiveInstance { get; private set; }
    public bool isCompleted { get; private set; } = false;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Craft";
    public Material highlightMaterial;

    [Header("Window Settings")]
    [SerializeField] private WindowType windowType = WindowType.SnakeMinigame;

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private Animator animator;
    private bool isOpen = false;
    // Lokaler Status wird durch den PauseMenuController verwaltet
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);

    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        originalMaterials = objectRenderer.materials;
    }

    public void Update()
    {
        // Synchronisiere das GameObject mit dem Window-Status
        //if (caftingUI != null && caftingUI.activeSelf != IsActive)
        //{
        //    caftingUI.SetActive(IsActive);
        //}
        //Debug.Log($"offen:" + isOpen);
    }

    public void Interact()
    {
        if (isCompleted)
        {
            Debug.Log($"{gameObject.name} Puzzle bereits abgeschlossen.");
            return;
        }

        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            PauseMenuController.Instance.CloseWindow();
            isOpen = false;
            return;
        }

        // Fenster öffnen
        PauseMenuController.Instance.OpenWindow(windowType);

        // Diese Instanz merken
        ActiveInstance = this;

        // PuzzleGameManager starten
        PictureGameManager ui = FindObjectOfType<PictureGameManager>(true);
        if (ui != null)
        {
            ui.InitGame();
        }

        isOpen = true;
    }
    public void EndGame()
    {
        if (isCompleted) return;

        isCompleted = true;
        Debug.Log($"{gameObject.name} Puzzle abgeschlossen!");

        // Interaktionstext löschen
        interactionText = "";
        PictureGameCountManager.Instance.AddCompletedPuzzle();

        // Aktive Instanz zurücksetzen
        if (ActiveInstance == this)
            ActiveInstance = null;

        // Fenster schließen (optional)
        isCompleted = true;
        used = true; // Assuming 'used' is a field in this class to track interaction state
        isEnabled = false;
        Remove();
        PauseMenuController.Instance.CloseWindow();
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