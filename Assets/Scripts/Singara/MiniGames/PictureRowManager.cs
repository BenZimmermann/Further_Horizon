using UnityEngine;
using UnityEngine.UI;

public class PuzzleRow : MonoBehaviour
{
    public Image rowImage;
    public Button leftButton;
    public Button rightButton;

    private Sprite[] options;   // hier sind ALLE 5 Teile drin
    private int currentIndex = 0;

    public int CorrectIndex { get; private set; } // Welches Teil gehört hierher?

    public void Init(Sprite[] allParts, int correctIndex)
    {
        options = allParts;
        CorrectIndex = correctIndex;

        // Zufälliges Startbild (NICHT unbedingt das richtige)
        currentIndex = Random.Range(0, options.Length);
        rowImage.sprite = options[currentIndex];

        leftButton.onClick.AddListener(() => Change(-1));
        rightButton.onClick.AddListener(() => Change(1));
    }


    void Change(int dir)
    {
        currentIndex = (currentIndex + dir + options.Length) % options.Length;
        rowImage.sprite = options[currentIndex];

        PictureGameManager.Instance.CheckWin();
    }

    public bool IsCorrect()
    {
        return currentIndex == CorrectIndex;
    }
}
