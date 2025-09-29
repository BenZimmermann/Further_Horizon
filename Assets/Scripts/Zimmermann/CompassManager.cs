using UnityEngine;
using TMPro;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.SocialPlatforms;

public class CompassManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform compassBar;            // The UI bar that acts as the compass
    public RectTransform directionMarkerPrefab; // Prefab for N/E/S/W direction markers
    public RectTransform objectiveMarker;       // Marker for the quest objective   
    public TMP_Text distanceText;               // Displays distance to the objective
    [SerializeField] private Transform spawnPoint;
    // Distanzanzeige für Objective

    [Header("World References")]
    public Transform playerCamera;          // Kamera oder Spieler
    public Transform objective;             // Quest-Ziel

    // Internal class to hold compass direction data
    private class CompassDirection
    {
        public string label;            // Text label (N, E, S, W)
        public Vector3 worldDir;        // World direction vector
        public RectTransform uiMarker;  
        public TMP_Text text;
    }
    // Array of directions (N, E, S, W)
    private CompassDirection[] directions;

    private void Start()
    {
        // Define the 4 main cardinal directions relative to world space
        directions = new CompassDirection[]
        {
            new CompassDirection { label = "N", worldDir = Vector3.forward },
            new CompassDirection { label = "E", worldDir = Vector3.right },
            new CompassDirection { label = "S", worldDir = Vector3.back },
            new CompassDirection { label = "W", worldDir = Vector3.left }
        };

        // Instantiate a UI marker for each direction and set its label
        foreach (var dir in directions)
        {
            RectTransform marker = Instantiate(directionMarkerPrefab, spawnPoint);
            dir.uiMarker = marker;
            dir.text = marker.GetComponentInChildren<TMP_Text>();
            dir.text.text = dir.label;
            // Position the marker on the compass bar
            SetMarkerPosition(dir.uiMarker, dir.worldDir);
        }
    }

    private void Update()
    {
        // Update the quest objective marker if we have one
        if (objective != null && objectiveMarker != null)
        {
            SetMarkerPosition(objectiveMarker, (objective.position - playerCamera.position).normalized);

            // Show distance in meters
            if (distanceText != null)
            {
                float dist = Vector3.Distance(playerCamera.position, objective.position);
                distanceText.text = $"{dist:F0}m";
            }
        }

        // Update the positions of the N/E/S/W markers each frame
        foreach (var dir in directions)
        {
            SetMarkerPosition(dir.uiMarker, dir.worldDir);
        }
    }
    /// <summary>
    /// Places a marker on the compass bar based on a world direction or target vector.
    /// </summary>
    private void SetMarkerPosition(RectTransform marker, Vector3 worldDirectionOrTarget)
    {
        //Flatten camera forward vector onto XZ plane(ignore vertical tilt)
        Vector3 forward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        // Flatten target direction onto XZ plane
        Vector3 flatDir = new Vector3(worldDirectionOrTarget.x, 0, worldDirectionOrTarget.z).normalized;

        // Signed Angle: -180 bis +180
        float angle = Vector3.SignedAngle(forward, flatDir, Vector3.up);

        // Normalize angle to range -1 -> +1
        float normalized = angle / 180f;
        float xPos = (compassBar.rect.width) * normalized;

        marker.anchoredPosition = new Vector2(xPos, marker.anchoredPosition.y);
    }
}
