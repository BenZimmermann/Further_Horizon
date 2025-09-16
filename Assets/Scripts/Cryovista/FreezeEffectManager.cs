using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FreezeEffectManager : MonoBehaviour
{
    public static FreezeEffectManager Instance { get; private set; }
    public ScriptableRendererFeature fullScreenPassFeature; // Reference to your Full Screen Pass Renderer Feature

    private bool isActive = true; // merkt sich den Status

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("FreezeEffectManager Instance created: " + name);
        }
        else
        {
            Debug.LogWarning("Duplicate FreezeEffectManager destroyed: " + name);
            Destroy(gameObject);
        }
    }

    public void ToggleFullScreenPass()
    {
        if (fullScreenPassFeature == null)
        {
            Debug.LogError("fullScreenPassFeature is NOT assigned!");
            return;
        }

        isActive = !isActive; // toggle Zustand
        fullScreenPassFeature.SetActive(isActive);

        Debug.Log("Setting FullScreenPassFeature to " + isActive);
    }

    public void SetFullScreenPass(bool enable)
    {
        if (fullScreenPassFeature == null)
        {
            Debug.LogError("fullScreenPassFeature is NOT assigned!");
            return;
        }

        isActive = enable;
        fullScreenPassFeature.SetActive(enable);

        Debug.Log("FullScreenPassFeature set to " + enable);
    }
}
