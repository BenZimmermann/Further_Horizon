using UnityEngine;

public interface Interactable
{
    public abstract void Awake();
    public abstract void Interact();
    public abstract void Apply();
    public abstract void Remove();
    public abstract string GetInteractionText();
    public abstract bool IsInteractable();
}