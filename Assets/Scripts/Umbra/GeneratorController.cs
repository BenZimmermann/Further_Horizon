using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public static GeneratorController Instance { get; private set; }

    public int totalGenerators;     // Gesamtanzahl, im Inspector oder automatisch setzen
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
        totalGenerators = FindObjectsOfType<GeneratorClass>().Length;
    }

    public void RegisterGenerator()
    {
        activeGenerators++;

        if (activeGenerators >= totalGenerators)
        {
            AllGeneratorsActivated();
        }
    }

    private void AllGeneratorsActivated()
    {
        Destroy(gameObject);
    }
}
