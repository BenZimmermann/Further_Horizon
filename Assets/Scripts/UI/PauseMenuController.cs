//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.SceneManagement;
//using static UnityEngine.InputSystem.InputAction;

//public enum WindowType
//{
//    PauseMenu,
//    Console,
//    Crafting,
//    Settings
//}

//public class PauseMenuController : MonoBehaviour
//{
//    [Header("Singleton")]
//    public static PauseMenuController Instance { get; private set; }
//    public static bool IsPaused { get; private set; }

//    public event System.Action OnWindowsClosed;

//    [Header("Pause Menu Settings")]
//    [SerializeField] private Camera mainCamera;
//    [SerializeField] private MonoBehaviour cameraController;

//    [Header("Window Settings")]
//    [SerializeField] private List<WindowEntry> windows;

//    [System.Serializable]
//    public struct WindowEntry
//    {
//        public WindowType type;
//        public GameObject windowObject;
//    }

//    private WindowType? currentActiveWindow = null;
//    private bool isAnyWindowOpen => currentActiveWindow.HasValue;
//    private bool isWindowOpen = false;
//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//        isWindowOpen = false;
//    }


//    public bool OpenWindow(WindowType windowType)
//    {

//        if (currentActiveWindow == windowType)
//            return false;

//        PauseGame();

//        currentActiveWindow = windowType;
//        GameObject obj = GetWindowByType(windowType);
//        if (obj != null)
//            Debug.Log("Opening window: " + windowType);
//        obj.SetActive(true);

//        return true;
//    }

//    public bool CloseWindow(WindowType windowType)
//    {
//        currentActiveWindow = windowType;
//        GameObject obj = GetWindowByType(windowType);
//        if (obj != null)
//            Debug.Log("Closing window: " + windowType);
//        obj.SetActive(false);
//        currentActiveWindow = null;
//        ResumeGame();

//        return true;
//    }


//    public bool IsWindowOpen(WindowType windowType)
//    {
//        return currentActiveWindow == windowType;
//    }

//    public GameObject GetWindowByType(WindowType type)
//    {
//        foreach (var pair in windows)
//        {
//            if (pair.type == type)
//                return pair.windowObject;
//        }
//        return null;
//    }



//    public void PauseGame()
//    {
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//        Time.timeScale = 0f;

//        if(cameraController != null)
//            cameraController.enabled = true;

//    }

//    private void ResumeGame()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//        Time.timeScale = 1f;

//        if (cameraController != null)
//            cameraController.enabled = true;
//    }
//}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WindowType
{
    PauseMenu,
    Console,
    Crafting,
    Settings,
    SnakeMinigame,
    PictureMinigame,
    Inventar
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
        DontDestroyOnLoad(gameObject);

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

    private void PauseGame()
    {
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

    private void ResumeGame()
    {
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
}
