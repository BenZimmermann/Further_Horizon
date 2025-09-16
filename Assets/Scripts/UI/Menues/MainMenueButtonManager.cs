using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    public void NewGamePressed()
    {
        Debug.Log($"{Pressed("New Game")}");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OptionsPressed()
    {
        var text = Pressed("Options");
        SceneManager.LoadScene(gameSceneName);
        Debug.Log(text);
    }
    private string Pressed(string button)
    {
        return button + " Button Pressed";
    }

    public void QuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}