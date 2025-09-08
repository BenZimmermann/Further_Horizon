using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;
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
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Idle", true);
        originalMaterials = objectRenderer.materials;
        if (animator == null) Debug.LogError("Animator not found on globe!");
    }

    public void Update()
    {
        // Synchronisiere das GameObject mit dem Window-Status
        if (map != null && map.activeSelf != IsActive)
        {
            map.SetActive(IsActive);
        }
        Debug.Log($"offen:"+ isOpen);

    }

    public void Interact()
    {
        // Wenn mein Fenster schon offen ist -> schließen
        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            animator.SetBool("Open", false);
            animator.SetBool("Idle", true);
            isOpen = false;
            return;

        
        }

        isOpen = true;
        // Fenster noch nicht offen -> erst Animation abspielen
        Debug.Log($"Console {windowType} wird geöffnet");
        PlayAnimation();
        StartCoroutine(OpenWindowAfterAnimation());
    }
    private IEnumerator OpenWindowAfterAnimation()
    {
        // Warte bis die Open-Animation durchgelaufen ist
        //float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(1.3f);
        PauseMenuController.Instance.OpenWindow(windowType);
        Debug.Log($"Console {windowType} geöffnet");
    }
    //private IEnumerator OpenWindow()
    //{
    //    yield return new WaitForSeconds(1);
    //    bool wasOpened = PauseMenuController.Instance.ToggleWindow(windowType);

    //    // Animation abspielen

    //    // Optional: Zusätzliche Aktionen beim Öffnen/Schließen
    //    if (wasOpened)
    //    {
    //        Debug.Log($"Console {windowType} geöffnet");

    //        // Hier können weitere Aktionen beim Öffnen hinzugefügt werden
    //    }
    //    else
    //    {
    //        Debug.Log($"Console {windowType} geschlossen");
    //        // Hier können weitere Aktionen beim Schließen hinzugefügt werden
    //    }
    //}

    private void PlayAnimation()
    {
            // Bool-Parameter Methode (empfohlen)
        animator.SetBool("Idle", false);
        animator.SetBool("Open", true);
            Debug.Log("Play Open Animation");

        // Alternative: Direkte Animation-Kontrolle
        // string animationName = isOpening ? "OpenAnimation" : "CloseAnimation";
        // childAnimator.Play(animationName, 0, 0f); // Start von Beginn an
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