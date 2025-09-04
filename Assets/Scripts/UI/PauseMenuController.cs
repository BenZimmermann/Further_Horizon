using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public enum WindowType
{
    PauseMenu,
    Console,
   // Map,
   //Inventory,
    Settings
}

public class PauseMenuController : MonoBehaviour
{
    [Header("Singleton")]
    public static PauseMenuController Instance { get; private set; }
    public static bool IsPaused { get; private set; }

    [Header("Pause Menu Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private MonoBehaviour cameraController;

    private PlayerInput playerInput;

    // Skalierbare Fensterverwaltung
    private WindowType? currentActiveWindow = null;
    private bool isAnyWindowOpen => currentActiveWindow.HasValue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Pause"].performed += onPause;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void onPause(CallbackContext ctx)
    {
        Debug.Log("Pause gedrückt");

        // ESC hat höchste Priorität
        if (isAnyWindowOpen)
        {
            if (currentActiveWindow == WindowType.PauseMenu)
            {
                CloseWindow();
            }
            else
            {
                CloseWindow();
                OpenWindow(WindowType.PauseMenu);
            }
        }
        else
        {
            OpenWindow(WindowType.PauseMenu);
        }
    }

    public bool OpenWindow(WindowType windowType)
    {
        // Wenn bereits dasselbe Fenster offen ist, tue nichts
        if (currentActiveWindow == windowType)
            return false;

        // Schließe aktuelles Fenster falls offen
        if (isAnyWindowOpen)
        {
            CloseCurrentWindow();
        }

        // Öffne neues Fenster
        currentActiveWindow = windowType;

        // Fenster-spezifische Aktionen
        switch (windowType)
        {
            case WindowType.PauseMenu:
                if (pauseCanvas != null)
                    pauseCanvas.SetActive(true);
                break;
                // Hier können weitere Fenstertypen hinzugefügt werden
                // case WindowType.Inventory:
                //     if (inventoryCanvas != null)
                //         inventoryCanvas.SetActive(true);
                //     break;
        }

        PauseGame();
        return true;
    }

    public bool CloseWindow()
    {
        if (!isAnyWindowOpen)
            return false;

        CloseCurrentWindow();
        currentActiveWindow = null;
        ResumeGame();
        return true;
    }

    public bool ToggleWindow(WindowType windowType)
    {
        if (currentActiveWindow == windowType)
        {
            CloseWindow();
            return false;
        }
        else
        {
            return OpenWindow(windowType);
        }
    }

    public bool IsWindowOpen(WindowType windowType)
    {
        return currentActiveWindow == windowType;
    }

    public bool IsAnyWindowOpen()
    {
        return isAnyWindowOpen;
    }

    public WindowType? GetCurrentActiveWindow()
    {
        return currentActiveWindow;
    }

    public bool CanOpenWindow(WindowType windowType)
    {
        // Zusätzliche Logik kann hier hinzugefügt werden
        // z.B. Prüfung auf spezielle Bedingungen für bestimmte Fenstertypen
        return true;
    }

    private void CloseCurrentWindow()
    {
        if (!currentActiveWindow.HasValue)
            return;

        // Fenster-spezifische Schließ-Aktionen
        switch (currentActiveWindow.Value)
        {
            case WindowType.PauseMenu:
                if (pauseCanvas != null)
                    pauseCanvas.SetActive(false);
                break;
                // Hier können weitere Fenstertypen hinzugefügt werden
        }
    }

    private void PauseGame()
    {
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        if (cameraController != null)
            cameraController.enabled = false;
    }

    private void ResumeGame()
    {
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        if (cameraController != null)
            cameraController.enabled = true;
    }

    // UI Button Methods
    public void ContinoueButtonPressed()
    {
        CloseWindow();
    }

    public void QuitButtonPressed()
    {
        Debug.Log("QuitButtonPressed aufgerufen");
        Time.timeScale = 1f;
    }


    //UI Button Methods ende
    public void RegisterExternalWindow(bool open)
    {
        if (open)
        {
            OpenWindow(WindowType.Console); // Standard für externe Fenster
        }
        else
        {
            CloseWindow();
        }
    }
}