using UnityEngine;

//same system as used in crownfall
public interface Interactable
{
    public abstract void Awake();
    public abstract void Update();
    public abstract void Interact();
    public abstract void Apply();
    public abstract void Remove();
    public abstract string GetInteractionText();
    public abstract bool IsInteractable();
}