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
    [SerializeField] private PlanetDefinition[] planetDefinitions; // alle verfügbaren Planeten
    [SerializeField] private Button[] planetButtons; // Buttons aus deinem ButtonHolder

    private PlanetDefinition selectedPlanet;
    private GameData gameData;

    private void Start()
    {
        // Button-Listener verbinden
        for (int i = 0; i < planetButtons.Length; i++)
        {
            int index = i; // Closure fix
            planetButtons[i].onClick.AddListener(() => SelectPlanet(planetDefinitions[index]));
        }

        startMissionButton.onClick.AddListener(StartMission);
    }

    public void LoadData(GameData data)
    {
        this.gameData = data;

        // letzte Auswahl laden oder Default
        selectedPlanet = planetDefinitions.FirstOrDefault(p => p.sceneName == data.selectedPlanetScene);

        if (selectedPlanet == null)
            selectedPlanet = planetDefinitions.FirstOrDefault();

        UpdateUI();
    }

    public void SaveData(GameData data)
    {
        if (selectedPlanet != null)
            data.selectedPlanetScene = selectedPlanet.sceneName;
    }

    private void SelectPlanet(PlanetDefinition planet)
    {
        // Nur auswählen, wenn Planet unlocked ist
        if (planet.isLocked || (gameData != null && !gameData.unlockedPlanets.Contains(planet.sceneName)))
        {
            Debug.Log($"Planet {planet.displayName} ist gesperrt!");
            return;
        }

        selectedPlanet = planet;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (selectedPlanet == null) return;

        planetNameTMP.text = selectedPlanet.displayName;
        planetDescriptionTMP.text = selectedPlanet.description;

        // Button aktiv/inaktiv je nach Locked
        bool unlocked = !selectedPlanet.isLocked &&
                        (gameData == null || gameData.unlockedPlanets.Contains(selectedPlanet.sceneName));
        startMissionButton.interactable = unlocked;
    }

    private void StartMission()
    {
        if (selectedPlanet == null) return;

        // Speichern vor Szenenwechsel
        DataPersistenceManager.Instance.SaveGame();

        Debug.Log($"Lade Mission: {selectedPlanet.sceneName}");
        SceneManager.LoadScene(selectedPlanet.sceneName);
    }
}
