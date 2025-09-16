using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestObject : MonoBehaviour, Interactable
{
    [Header("Quest Info")]
    [SerializeField] private string questName;
    [SerializeField] private string description;

    private bool activated = false;
    public void Awake() { }
    public void Update() { }

    public void Apply() { }
    public void Remove() { }

    public string GetInteractionText()
    {
        return activated ? "" : $"Starte Quest: {questName}";
    }

    public bool IsInteractable()
    {
        return !activated;
    }

    public void Interact()
    {
        if (activated) return;
        activated = true;

        Debug.Log($"Quest gestartet: {questName} - {description}");

        // TODO: QuestManager informieren
        Destroy(gameObject); // oder stehen lassen
    }
}
