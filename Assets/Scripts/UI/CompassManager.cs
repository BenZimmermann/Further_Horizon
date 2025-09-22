using UnityEngine;
using TMPro;

public class CompassManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform compassBar;        // Die UI-Leiste für den Kompass
    public RectTransform directionMarkerPrefab; // Prefab für N/E/S/W Marker (mit Text)
    public RectTransform objectiveMarker;   // Marker für das Quest-Objective
    public TMP_Text distanceText;
    [SerializeField] private Transform spawnPoint;
    // Distanzanzeige für Objective

    [Header("World References")]
    public Transform playerCamera;          // Kamera oder Spieler
    public Transform objective;             // Quest-Ziel

    // interne Struktur für Himmelsrichtungen
    private class CompassDirection
    {
        public string label;
        public Vector3 worldDir;
        public RectTransform uiMarker;
        public TMP_Text text;
    }

    private CompassDirection[] directions;

    private void Start()
    {
        // Haupt-Himmelsrichtungen definieren
        directions = new CompassDirection[]
        {
            new CompassDirection { label = "N", worldDir = Vector3.forward },
            new CompassDirection { label = "E", worldDir = Vector3.right },
            new CompassDirection { label = "S", worldDir = Vector3.back },
            new CompassDirection { label = "W", worldDir = Vector3.left }
        };

        // Marker im UI instanziieren
        foreach (var dir in directions)
        {
            RectTransform marker = Instantiate(directionMarkerPrefab, spawnPoint);
            dir.uiMarker = marker;
            dir.text = marker.GetComponentInChildren<TMP_Text>();
            dir.text.text = dir.label;
            SetMarkerPosition(dir.uiMarker, dir.worldDir);
        }
    }

    private void Update()
    {


        // Objective aktualisieren
        if (objective != null && objectiveMarker != null)
        {
            SetMarkerPosition(objectiveMarker, (objective.position - playerCamera.position).normalized);

            // Distanz anzeigen
            if (distanceText != null)
            {
                float dist = Vector3.Distance(playerCamera.position, objective.position);
                distanceText.text = $"{dist:F0}m";
            }
        }

        // Himmelsrichtungen aktualisieren
        foreach (var dir in directions)
        {
            SetMarkerPosition(dir.uiMarker, dir.worldDir);
        }
    }

    private void SetMarkerPosition(RectTransform marker, Vector3 worldDirectionOrTarget)
    {
        // Wenn es ein Richtungsvektor ist, ist er schon normiert
        // Wenn es ein World-Target ist, wird im Aufruf bereits normalisiert
        Vector3 forward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        Vector3 flatDir = new Vector3(worldDirectionOrTarget.x, 0, worldDirectionOrTarget.z).normalized;

        // Signed Angle: -180 bis +180
        float angle = Vector3.SignedAngle(forward, flatDir, Vector3.up);

        // Auf -1 bis +1 normieren
        float normalized = angle / 180f;
        float xPos = (compassBar.rect.width) * normalized;

        marker.anchoredPosition = new Vector2(xPos, marker.anchoredPosition.y);
    }
}
