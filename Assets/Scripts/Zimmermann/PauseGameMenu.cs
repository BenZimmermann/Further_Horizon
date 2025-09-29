using UnityEngine;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseGameMenu : MonoBehaviour
{
    [Header("Window Settings")]
    [SerializeField] private string newGameScene;   // Name der Startszene fürs neue Spiel
    [SerializeField] private WindowType windowType = WindowType.PauseMenu;
    private bool IsActive => PauseMenuController.Instance.IsWindowOpen(windowType);
    private bool isOpen = false;
    private PlayerInput playerInput;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Pause"].performed += onPause;
    }


    private void onPause(CallbackContext ctx)
    {
        Debug.Log("Pause gedrückt");
        if (PauseMenuController.Instance.IsWindowOpen(windowType))
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            //PauseMenuController.Instance.CloseWindow(windowType);
            isOpen = false;
            return;
        }

        Debug.Log($"Console {windowType} wird geöffnet");
        PauseMenuController.Instance.OpenWindow(windowType);
        isOpen = true;
    }
    public void OnContiouePressed() {
        if (isOpen)
        {
            Debug.Log($"Console {windowType} geschlossen");
            PauseMenuController.Instance.CloseWindow();
            //PauseMenuController.Instance.CloseWindow(windowType);
            isOpen = false;
            return;
        }
    }
    public void OnbackToShip()
    {
        if (isOpen)
        {
            Debug.Log("ich wurde gedrückt");
            PauseMenuController.Instance.CloseWindow();
            SceneManager.LoadScene(newGameScene);
            isOpen = false;
            return;
        }
    }
}
