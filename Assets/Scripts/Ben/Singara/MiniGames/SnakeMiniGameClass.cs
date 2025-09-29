using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;
public class SnakeMiniGameClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;
    //private bool used = false;
    public bool isCompleted { get; private set; } = false;

    [Header("Interaction Settings")]
    public string interactionText = "(F) Craft";
    public Material highlightMaterial;

    [Header("Window Settings")]
    [SerializeField] private WindowType windowType = WindowType.SnakeMinigame;

    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private Animator animator;
   // private bool isOpen = false;
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
        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            //PauseMenuController.Instance.CloseWindow(windowType);
            //isOpen = false;
            return;
        }

        Debug.Log($"Console {windowType} wird geöffnet");
        PauseMenuController.Instance.OpenWindow(windowType);
        SnakeLabyrinthUI.Instance.InitGame(this);

       // isOpen = true;
    }
    public void EndGame()
    {
        if (!isCompleted)
        {
            SnakeGameCountManager.Instance.AddCompletedGame();
            isCompleted = true;
            //used = true; 
            isEnabled = false;
            Remove();
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