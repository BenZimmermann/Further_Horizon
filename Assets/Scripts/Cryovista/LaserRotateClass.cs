using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RotateDirection
{
    X,
    Y
}

// erbt von Interactable, um Interaktionen zu ermöglichen
public class LaserRotateClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Rotation Settings")]
    [SerializeField] private float rotateAngle = 45f;          // Winkel pro Interaktion
    [SerializeField] private RotateDirection rotationDirection; // Auswahl im Inspector

    [Header("Interaction Settings")]
    public string interactionText = "(F) Rotate";
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
        if (!isEnabled) return;

        Vector3 rotation = Vector3.zero;

        switch (rotationDirection)
        {
            case RotateDirection.X:
                rotation = new Vector3(rotateAngle, 0, 0);
                break;
            case RotateDirection.Y:
                rotation = new Vector3(0, rotateAngle, 0);
                break;
        }

        transform.Rotate(rotation, Space.Self);
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