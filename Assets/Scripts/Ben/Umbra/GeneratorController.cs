using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public static GeneratorController Instance { get; private set; }

    public int totalGenerators; 
    private int activeGenerators = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Count all generators in the scene at start
        totalGenerators = FindObjectsOfType<GeneratorClass>().Length;
    }

    /// <summary>
    /// Called when a generator is activated.
    /// Tracks how many generators are active.
    /// </summary>
    public void RegisterGenerator()
    {
        activeGenerators++;

        // If all generators are activated, trigger completion
        if (activeGenerators >= totalGenerators)
        {
            AllGeneratorsActivated();
        }
    }

    /// <summary>
    /// Called when all generators are activated.
    /// Can trigger events, open doors, or destroy this controller if no longer needed.
    /// </summary>
    private void AllGeneratorsActivated()
    {
        Destroy(gameObject);
    }
}
