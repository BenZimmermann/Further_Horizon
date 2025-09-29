using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FreezeEffectManager : MonoBehaviour
{
    public static FreezeEffectManager Instance { get; private set; }
    public ScriptableRendererFeature fullScreenPassFeature; // Reference to Full Screen Pass Renderer Feature

    private bool isActive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// toggle the current screen effect on
    /// </summary>
    public void ToggleFullScreenPass()
    {
        if (fullScreenPassFeature == null)
        {
            return;
        }

        isActive = !isActive; // toggle
        //theorie
        fullScreenPassFeature.SetActive(isActive);
    }

    public void SetFullScreenPass(bool enable)
    {
        if (fullScreenPassFeature == null)
        {
            return;
        }

        isActive = enable;
        fullScreenPassFeature.SetActive(enable);

        Debug.Log("FullScreenPassFeature set to " + enable);
    }
}
