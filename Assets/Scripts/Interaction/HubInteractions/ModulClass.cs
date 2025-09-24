using UnityEngine;
using UnityEngine.UI;
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

    // NEU: Welches Modul-Item wird für die Aktivierung benötigt?
    [Header("Required Inventory Item")]
    [SerializeField] private ItemDefinition requiredModuleItem;
    [SerializeField] private int requiredAmount = 1;
    public int RequiredAmount => requiredAmount;

    // Ende neu.

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

    [Header("UI (optional)")]
    [SerializeField] private Image slotImage;             // Bild im Slot, wenn installiert
    [SerializeField] private Button installButton;        // Button zum Installieren (optional)
    [SerializeField] private Sprite installedSprite;      // Sprite, wenn installiert

    [Header("Erforderliches Modul-Item")]
    [Tooltip("Das fertige Modul-Item aus dem Inventar (z. B. Heatexchanger).")]
    [SerializeField] private ItemDefinition moduleItem;

    [Header("Optionale Quest-Abhängigkeiten")]
    [Tooltip("Alle diese Quests müssen abgeschlossen sein, damit installiert werden darf.")]
    [SerializeField] private string[] requiredQuestIds;

    [Header("Optionale Zusatz-Items")]
    [SerializeField] private ItemDefinition[] additionalRequiredItems;

    // Zustand
    private bool _isInstalled;
    public void Awake()
    {
        // Button verdrahten (falls gesetzt)
        if (installButton != null)
            installButton.onClick.AddListener(TryInstall);

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
    private void Start()
    {
        RefreshUI();
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
        // NEU YUSUF: Wenn ein Item benötigt wird, aber nicht vorhanden ist -> Hinweis zurück 
        if (requiredModuleItem != null)
        {
            var inv = InventoryManager.Instance;
            if (inv != null && !inv.HasEnoughItems(requiredModuleItem, requiredAmount))
                return $"{interactionText} (needs: {requiredModuleItem.displayName} x{requiredAmount})";
        }

        // Standardtext aus Awake
        return interactionText;
    }
    public void Update()
    {
    }
    #region Interact Bereich für Moduls
    public void Interact()
    {
        if (used) return;
        // 1) QuestManager benachrichtigen
        var questManager = GetComponent<QuestObject>();
        if (questManager != null)
        {
            // required 
            if (!questManager.IsInteracted()) return;
        }
        // 2) Spieler darf Modul noch nicht aktivieren
        if (!CheckActivationPermission(_ModuleType))
        {
            Debug.Log($"{_ModuleType} kann nicht aktiviert werden!");
            StartCoroutine(ShowDeniedWindow());
            return;
        }

        /// Hab ich rausgemacht weils mir probleme gemacht hat, neue Variable neues Glück mein Motto
        //// 3) Maximal 3 Module aktivierbar
        //if (useCount >= 3)
        //{
        //    Debug.Log("Es können keine weiteren Module aktiviert werden.");
        //    used = true;
        //    return;
        //}

        // 4) Prüfen ob dieses Modul schon aktiviert wurde
        if (activatedModules.Contains(_ModuleType))
        {
            Debug.Log($"{_ModuleType} Modul wurde bereits aktiviert.");
            return;
        }

        // 5) Inventar-Prüfung (NEU Yusuf): ohne Item keine Aktivierung
        if (requiredModuleItem != null)
        {
            var inv = InventoryManager.Instance;
            if (inv == null)
            {
                Debug.LogError("[Module] Kein InventoryManager gefunden!");
                return;
            }

            if (!inv.TryConsume(requiredModuleItem, requiredAmount))
            {
                Debug.Log($"[Module] Benötigt: {requiredModuleItem.displayName} x{requiredAmount}");
                StartCoroutine(ShowDeniedWindow());
                return;
            }
        }

        // 6) Modul aktivieren
        Debug.Log($"{_ModuleType} Modul aktiviert");
        activatedModules.Add(_ModuleType);
        useCount++;
        used = true;

        // 7) NEU Yusuf -> auch in GameData hinterlegen
        var dpm = DataPersistenceManager.Instance;
        if (dpm != null)
        {
            var gd = dpm.GetGameData();
            if (gd != null && !gd.installedModuleIds.Contains(_ModuleType.ToString()))
            {
                gd.installedModuleIds.Add(_ModuleType.ToString());

                // Zusätzlich: QuestItem/ModulItem sperren -> Kommt cooler 
                if (requiredModuleItem != null)
                {
                    gd.pendingInstallModuleIds.Remove(requiredModuleItem.itemId);
                }

                dpm.SaveGame();
            }
        }

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

        //// Hier wird sofort gespeichert, damit Aktivierung + Item-Entnahme sicher im Save landen
        /// Der Teil ist nicht mehr nötig hab ich in Schritt 7 eingebaut, nicht wundern
        //var dpm = DataPersistenceManager.Instance;
        //if (dpm != null) dpm.SaveGame();
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
    #endregion 

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

    // ---- Public API (falls du keinen Button nutzt, kannst du diesen Call z.B. über Interact auslösen)
    public void TryInstall()
    {
        if (_isInstalled) return;
        if (!CanInstall()) return;

        var inv = InventoryManager.Instance;
        if (inv == null) return;

        // 1) Modul-Item konsumieren
        if (!inv.TryConsume(moduleItem, requiredAmount))
        {
            Debug.LogWarning("[Modul] Modul-Item konnte nicht verbraucht werden.");
            return;
        }

        // 2) Zusatz-Items konsumieren (optional)
        if (additionalRequiredItems != null)
        {
            foreach (var need in additionalRequiredItems)
            {
                if (need == null) continue;
                if (!inv.TryConsume(need, 1))
                {
                    Debug.LogWarning("[Modul] Zusatzitem konnte nicht verbraucht werden.");
                    // Optional: Rollback wäre hier möglich, wenn du willst.
                }
            }
        }

        // 3) Installiert markieren & speichern
        _isInstalled = true;
        PersistInstalledModule();
        RefreshUI();

        Debug.Log($"[Modul] '{moduleItem?.displayName}' installiert.");
    }

    // ---- intern

    private bool CanInstall()
    {
        if (moduleItem == null) return false;
        if (_isInstalled) return false;

        // a) Quests erfüllt? (wenn QuestManager existiert und IDs angegeben sind)
        if (requiredQuestIds != null && requiredQuestIds.Length > 0)
        {
            var qm = QuestManager.Instance; // falls nicht vorhanden -> Quests werden ignoriert
            if (qm != null)
            {
                foreach (var q in requiredQuestIds)
                {
                    if (string.IsNullOrWhiteSpace(q)) continue;
                    if (!qm.IsQuestCompleted(q))
                        return false;
                }
            }
        }

        // b) Inventar-Check
        var inv = InventoryManager.Instance;
        if (inv == null) return false;

        if (!inv.HasEnoughItems(moduleItem, requiredAmount))
            return false;

        if (additionalRequiredItems != null)
        {
            foreach (var need in additionalRequiredItems)
            {
                if (need == null) continue;
                if (!inv.HasEnoughItems(need, 1))
                    return false;
            }
        }

        return true;
    }

    private void PersistInstalledModule()
    {
        var dpm = DataPersistenceManager.Instance;
        if (dpm == null) return;

        var gd = dpm.GetGameData();
        if (gd == null) return;

        // Modul per itemId merken
        if (moduleItem != null && !gd.installedModuleIds.Contains(moduleItem.itemId))
            gd.installedModuleIds.Add(moduleItem.itemId);

        dpm.SaveGame();
    }

    private void RefreshUI()
    {
        // Bild
        if (slotImage != null)
            slotImage.sprite = _isInstalled ? installedSprite : null;

        // Button
        if (installButton != null)
            installButton.interactable = !_isInstalled && CanInstall();
    }

    // ---- Aufruf vom DataPersistenceManager nach dem Laden (optional, falls du IDataPersistence nutzt)
    // Falls deine ModulClass NICHT IDataPersistence implementiert: ruf SetStateFromSave() einmal extern beim Laden auf.
    public void SetStateFromSave()
    {
        var dpm = DataPersistenceManager.Instance;
        if (dpm == null) { RefreshUI(); return; }

        var gd = dpm.GetGameData();
        if (gd == null) { RefreshUI(); return; }

        _isInstalled = (moduleItem != null) && gd.installedModuleIds.Contains(moduleItem.itemId);
        RefreshUI();
    }
}