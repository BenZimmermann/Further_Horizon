using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Test : MonoBehaviour
{
    [SerializeField] private FullScreenPassRendererFeature fullScreenPassRendererFeature;


    public void DisableFeature(bool active)
    {
        fullScreenPassRendererFeature.SetActive(active);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
