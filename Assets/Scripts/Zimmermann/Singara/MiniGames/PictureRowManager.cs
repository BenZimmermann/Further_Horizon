using UnityEngine;
using UnityEngine.UI;

public class PuzzleRow : MonoBehaviour
{
    public Image rowImage;          // UI Image component to display the current puzzle piece
    public Button leftButton;       // Button to cycle left through options
    public Button rightButton;      // Button to cycle right through options

    private Sprite[] options;   
    private int currentIndex = 0;

    // The correct index for this row (set during initialization)
    public int CorrectIndex { get; private set; }

    // Initialize this row with all puzzle pieces and the correct target piece index
    public void Init(Sprite[] allParts, int correctIndex)
    {
        options = allParts;
        CorrectIndex = correctIndex;

        // Start with a random puzzle piece (not necessarily the correct one)
        currentIndex = Random.Range(0, options.Length);
        rowImage.sprite = options[currentIndex];

        // Assign button listeners
        leftButton.onClick.AddListener(() => Change(-1));
        rightButton.onClick.AddListener(() => Change(1));
    }

    // Change the currently displayed puzzle piece
    void Change(int dir)
    {
        currentIndex = (currentIndex + dir + options.Length) % options.Length;
        rowImage.sprite = options[currentIndex];

        PictureGameManager.Instance.CheckWin();
    }

    // Check if this row is displaying the correct piece
    public bool IsCorrect()
    {
        return currentIndex == CorrectIndex;
    }
}
