using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private string newGameScene;   // Name der Startszene fürs neue Spiel
    [SerializeField] private string settingsScene;  // Name der Optionsszene

    public void NewGamePressed()
    {
        Debug.Log("[MainMenu] New Game pressed.");

        // Neue SaveGame anlegen erstmal vor Start
        DataPersistenceManager.Instance.NewGame();

        // Jetzt kommt HUB Szene als newGameScene Inspector net vergessen
        SceneManager.LoadScene(newGameScene);
    }
    public void OptionsPressed()
    {
        // Für Später falls ma sowas noch schaffen
        Debug.Log("[MainMenu] Options pressed.");
        //SceneManager.LoadScene(settingsScene);
    }

    public void LoadGamePressed()
    {
        // Spiel laden, gucken ob Button funktioniert
        Debug.Log("[MainMenu] Load Game pressed.");

        // Versuchen die gepeicherten Daten zu Laden 
        DataPersistenceManager.Instance.LoadGame();

        // Falls nix da sein sollte, im Hintergrund erstellen 
        // danach passende Szene aus dem Savegame starten
        var data = DataPersistenceManager.Instance.GetGameData();
        if (data != null && !string.IsNullOrEmpty(data.selectedPlanetScene))
        {
            SceneManager.LoadScene(data.selectedPlanetScene); // Hier muss ma ufpasse welcher Planet eingetragen ist als selected in der GameData.cs
        }
        else
        {
            // Fallback Falls irgendwas fehlt  auf newGameScene gehen
            SceneManager.LoadScene(newGameScene);
        }
    }

    public void QuitPressed() // Feierabend
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}