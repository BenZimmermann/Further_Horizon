using UnityEngine;
public class EnemyVisionCone : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewDistance;
    public float viewAngle;

    private Light visionLight;

    void Awake()
    {
        // Neues Spot Light erstellen, falls noch keins existiert
        visionLight = GetComponentInChildren<Light>();
        if (visionLight == null)
        {
            GameObject lightObj = new GameObject("VisionConeLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = new Vector3(0f, 1.8f, 0f); // Kopfhöhe
            lightObj.transform.localRotation = Quaternion.identity;

            visionLight = lightObj.AddComponent<Light>();
            visionLight.type = LightType.Spot;
            visionLight.shadows = LightShadows.None; // Performance
            visionLight.color = new Color(1f, 0f, 0f, 1f); // gelblich
            visionLight.intensity = 30f;
        }
    }

    void Update()
    {
        // Werte immer aktuell halten
        visionLight.range = viewDistance;
        visionLight.spotAngle = viewAngle * 2f; // da Unity den vollen Öffnungswinkel erwartet
    }
}

