using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum ModuleType
{
    Umbra,
    Singara,
    Cryovista,
}
public class ModulClass : MonoBehaviour, Interactable
{
    [Header("General Settings")]
    public bool isInteractable;
    public bool isEnabled = true;

    [Header("Interaction Settings")]
    [SerializeField] private string interactionText = "";
    public Material highlightMaterial;

    private Material[] originalMaterials;
    private Renderer objectRenderer;

    [Header("Module Settings")]
    public ModuleType _ModuleType;
    [SerializeField] private GameObject deniedWindow;

    private static HashSet<ModuleType> activatedModules = new HashSet<ModuleType>();

    [Header("Activation Permissions")]
     private bool canActivateUmbra = true;
     private bool canActivateSingara = true;
     private bool canActivateCryovista = true;

    private bool used = false;
    private static int useCount = 0; // globaler Zähler für alle Module
    //private float useCount;
    [Header("Text Objects")]
    [SerializeField] GameObject UmbraTxtObj;
    [SerializeField] GameObject SingaraTxtObj;
    [SerializeField] GameObject CryovistaTxtObj;

    [Header("3D Module Objects")]
    [SerializeField] GameObject UmbraObj;
    [SerializeField] GameObject SingaraObj;
    [SerializeField] GameObject CryovistaObj;
    public void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;

        switch (_ModuleType)
        {
            case ModuleType.Umbra:
                interactionText = "(F) Activate Umbra";
                break;
            case ModuleType.Singara:
                interactionText = "(F) Activate Singara";
                break;
            case ModuleType.Cryovista:
                interactionText = "(F) Activate Cryovista";
                break;
        }
    }
    public string GetInteractionText()
    {
        // Wenn schon benutzt oder global aktiviert
        if (used || activatedModules.Contains(_ModuleType))
        {
            return $"{_ModuleType} already activated";
        }

        // Wenn Spieler keine Berechtigung hat
        if (!CheckActivationPermission(_ModuleType))
        {
            return $"{_ModuleType} cannot be activated";
        }

        // Standardtext aus Awake
        return interactionText;
    }
    public void Update()
    {
    }
    public void Interact()
    {
        if (used) return;

        // Spieler darf Modul noch nicht aktivieren
        if (!CheckActivationPermission(_ModuleType))
        {
            Debug.Log($"{_ModuleType} kann nicht aktiviert werden!");
            StartCoroutine(ShowDeniedWindow());
            return;
        }

        // Maximal 3 Module aktivierbar
        if (useCount >= 3)
        {
            Debug.Log("Es können keine weiteren Module aktiviert werden.");
            used = true;
            return;
        }

        // Prüfen ob dieses Modul schon aktiviert wurde
        if (activatedModules.Contains(_ModuleType))
        {
            Debug.Log($"{_ModuleType} Modul wurde bereits aktiviert.");
            return;
        }

        // Modul aktivieren
        Debug.Log($"{_ModuleType} Modul aktiviert");
        activatedModules.Add(_ModuleType);
        useCount++;
        used = true;

        Remove();
        Apply();

        if (_ModuleType == ModuleType.Umbra && UmbraObj != null)
        {
            Destroy(UmbraTxtObj);
            UmbraObj.SetActive(true);
        }
        if (_ModuleType == ModuleType.Singara && SingaraObj != null)
        {
            Destroy(SingaraTxtObj);
            SingaraObj.SetActive(true);
        }
        if (_ModuleType == ModuleType.Cryovista && CryovistaObj != null)
        {
            Destroy(CryovistaTxtObj);
            CryovistaObj.SetActive(true);
        }
        // ...
    }
    private bool CheckActivationPermission(ModuleType type)
    {
        switch (type)
        {
            case ModuleType.Umbra: return canActivateUmbra;
            case ModuleType.Singara: return canActivateSingara;
            case ModuleType.Cryovista: return canActivateCryovista;
            default: return false;
        }
    }

    private IEnumerator ShowDeniedWindow()
    {
        if (deniedWindow == null) yield break;
        deniedWindow.SetActive(true);
        yield return new WaitForSeconds(3f);
        deniedWindow.SetActive(false);
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

    //public string GetInteractionText()
    //{
    //    return interactionText;
    //}

    public bool IsInteractable()
    {
        //return !used;
        return !used && !activatedModules.Contains(_ModuleType);

    }
}