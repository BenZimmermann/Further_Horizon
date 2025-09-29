using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum WindowType
{
    PauseMenu,
    Console,
    Crafting,
    Settings,
    SnakeMinigame,
    PictureMinigame,
    Inventar,
    GameOver
}

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }

    [Header("Window Settings")]
    [SerializeField] private List<WindowEntry> windows; 
    [SerializeField] private List<GUI> guis;

    [SerializeField] private MonoBehaviour cameraController;

    [System.Serializable]
    public struct WindowEntry
    {
        public WindowType type;
        public GameObject windowObject;
    }
    [System.Serializable]
    public struct GUI
    {
        public GameObject guiObject;
    }
    private WindowType? currentActiveWindow = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // alle Fenster beim Start deaktivieren
        foreach (var entry in windows)
        {
            if (entry.windowObject != null)
                entry.windowObject.SetActive(false);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #region Window Input Handling
    public bool OpenWindow(WindowType windowType)
    {
        // Öffne nur, wenn aktuell kein Fenster offen ist
        if (currentActiveWindow != null)
            return false;

        var obj = GetWindowByType(windowType);
        if (obj != null)
        {
            obj.SetActive(true);
            currentActiveWindow = windowType;
            PauseGame();
            Debug.Log("Opening window: " + windowType);
            return true;
        }
        return false;
    }

    public bool CloseWindow()
    {
        if (currentActiveWindow == null)
            return false;

        var obj = GetWindowByType(currentActiveWindow.Value);
        if (obj != null)
        {
            obj.SetActive(false);
            Debug.Log("Closing window: " + currentActiveWindow.Value);
        }

        currentActiveWindow = null;
        ResumeGame();
        return true;
    }

    public bool IsWindowOpen(WindowType windowType)
    {
        return currentActiveWindow == windowType;
    }

    private GameObject GetWindowByType(WindowType type)
    {
        foreach (var entry in windows)
        {
            if (entry.type == type)
                return entry.windowObject;
        }
        return null;
    }
    #endregion Window Input Handling

    #region Spielzustand
    // Spiel pausieren
    private void PauseGame()
    {
        // Cursor und timescale setzen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        //alle fenster die in GUI list sind deaktivieren
        foreach (var gui in guis)
        {
            if (gui.guiObject != null)
                gui.guiObject.SetActive(false);
        }
        if (cameraController != null)
            cameraController.enabled = false;
    }
    // Spiel fortsetzen
    private void ResumeGame()
    {
        // Cursor und timescale setzen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        //alle fenster die in GUI list sind aktivieren
        foreach (var gui in guis)
        {
            if (gui.guiObject != null)
                gui.guiObject.SetActive(true);
        }
        if (cameraController != null)
            cameraController.enabled = true;
    }
    #endregion Spielzustand

    #region Szenen Wechsel Verwaltung Yusuf
    private void OnEnable() // Callback registrieren
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() // Callback deregistrieren
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Egal was vorher war: Spiel aktivieren
        ForceResumeAndClear();

        // Alle eventuell noch referenzierten Fenster aus alter Szene sicherheitshalber deaktivieren
        if (windows != null)
        {
            foreach (var entry in windows)
            {
                if (entry.windowObject != null)
                    entry.windowObject.SetActive(false);
            }
        }
    }

    public void ForceResumeAndClear()
    {
        currentActiveWindow = null;   // kein Fenster als offen markieren
        // Cursor/Timescale/Kamera sauber setzen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        // Alle GUI-Elemente wieder aktivieren
        if (guis != null) 
        {
            foreach (var gui in guis)
                if (gui.guiObject != null) gui.guiObject.SetActive(true);
        }
        if (cameraController != null)
            cameraController.enabled = true;
    }
    #endregion

}
