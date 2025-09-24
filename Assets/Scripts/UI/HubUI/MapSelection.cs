using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelection : MonoBehaviour, IDataPersistence
{
    [Header("UI Referenzen")]
    [SerializeField] private TextMeshProUGUI planetNameTMP;
    [SerializeField] private TextMeshProUGUI planetDescriptionTMP;
    [SerializeField] private Button startMissionButton;

    [Header("Planeten")]
    [SerializeField] private PlanetDefinition[] planetDefinitions; // alle verfügbaren Planeten (Reihenfolge = Buttons)
    [SerializeField] private Button[] planetButtons;               // Buttons im ButtonHolder (gleich lang wie planetDefinitions)

    [SerializeField] private string defaultPlanetScene = "Cryovista";


    private PlanetDefinition selectedPlanet;
    private GameData gameData;

    // ---------- Lifecycle ----------
    private void Start()
    {
        // Buttons zunächst sperren bis LoadData() gelaufen ist
        SetAllPlanetButtonsInteractable(false);
        if (startMissionButton) startMissionButton.interactable = false;

        // Button-Listener verbinden
        for (int i = 0; i < planetButtons.Length && i < planetDefinitions.Length; i++)
        {
            int index = i; // Closure-Fix
            planetButtons[i].onClick.AddListener(() => SelectPlanet(planetDefinitions[index]));
        }

        // Start-Listener
        startMissionButton.onClick.AddListener(StartMission);

        // Default-Planet sofort vorwählen und seinen Button freischalten,
        // damit er auch ohne LoadData() direkt klickbar ist.
        var defaultDef = planetDefinitions.FirstOrDefault(pd => pd.sceneName == defaultPlanetScene);
        if (defaultDef != null)
        {
            selectedPlanet = defaultDef;

            // Entsprechenden Button aktivieren (planetDefinitions & planetButtons gleiche Reihenfolge!)
            int idx = System.Array.IndexOf(planetDefinitions, defaultDef);
            if (idx >= 0 && idx < planetButtons.Length && planetButtons[idx] != null)
                planetButtons[idx].interactable = true;
        }


        // Leeres UI bis Daten da sind
        UpdateUI();
    }

    #region Speichern und Laden - IDataPersistence
    public void LoadData(GameData data)
    {
        gameData = data;

        // Buttons jetzt gemäß freigeschalteten Planeten setzen
        RefreshPlanetButtons();

        // Vorauswahl: gespeicherter Planet, aber nur wenn freigeschaltet
        selectedPlanet = planetDefinitions.FirstOrDefault(p =>
            p.sceneName == gameData.selectedPlanetScene && IsUnlocked(p));

        // Fallback: erster freigeschalteter Planet (bei NewGame -> Cryovista)
        if (selectedPlanet == null)
            selectedPlanet = planetDefinitions.FirstOrDefault(IsUnlocked);

        UpdateUI();
    }

    public void SaveData(GameData data)
    {
        if (selectedPlanet != null)
            data.selectedPlanetScene = selectedPlanet.sceneName;
    }

    #endregion

    #region Auswahl und UI 

    private void SelectPlanet(PlanetDefinition planet)
    {
        if (!IsUnlocked(planet))
        {
            Debug.Log($"[MapSelection] '{planet.displayName}' ist gesperrt.");
            return;
        }

        selectedPlanet = planet;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (selectedPlanet == null)
        {
            planetNameTMP.text = "Routenwähler";
            planetDescriptionTMP.text = "Wähle deinen Planeten den du bereisen möchtest";
            if (startMissionButton) startMissionButton.interactable = false;
            return;
        }

        planetNameTMP.text = selectedPlanet.displayName;
        planetDescriptionTMP.text = selectedPlanet.description;

        // Start nur, wenn der aktuell ausgewählte Planet freigeschaltet ist
        if (startMissionButton) startMissionButton.interactable = IsUnlocked(selectedPlanet);
    }

    private void RefreshPlanetButtons()
    {
        for (int i = 0; i < planetButtons.Length && i < planetDefinitions.Length; i++)
        {
            var def = planetDefinitions[i];
            planetButtons[i].interactable = IsUnlocked(def);
        }
    }

    private void SetAllPlanetButtonsInteractable(bool value)
    {
        foreach (var b in planetButtons)
            if (b) b.interactable = value;
    }

    private bool IsUnlocked(PlanetDefinition p)
    {
        if (p == null) return false;
        if (p.isLocked) return false;

        // Default-Planet ist immer freigeschaltet – völlig unabhängig von Save/Load.
        if (p.sceneName == defaultPlanetScene)
            return true;

        // Alle anderen Planeten gehen über GameData.unlockedPlanets
        if (gameData == null) return false;
        return gameData.unlockedPlanets != null
            && gameData.unlockedPlanets.Contains(p.sceneName);
    }


    // ---------- Start ----------
    private void StartMission()
    {
        if (selectedPlanet == null || !IsUnlocked(selectedPlanet))
            return;

        // Sicher speichern vor Szenenwechsel
        DataPersistenceManager.Instance.SaveGame();

        PauseMenuController.Instance?.ForceResumeAndClear(); // Zusatz um alle Fenster zu Schließen gegen Freeze Time

        Debug.Log($"[MapSelection] Lade Mission: {selectedPlanet.displayName} ({selectedPlanet.sceneName})");
        SceneManager.LoadScene(selectedPlanet.sceneName);
    }

    #endregion

}
