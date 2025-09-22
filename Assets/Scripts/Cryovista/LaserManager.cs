using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    public int maxReflections = 10;       //standardmäßig immer start bei 2;
    public float maxDistance = 10f;        // maximale Reichweite pro Strahl
    public LayerMask collisionLayers;      // welche Layer der Laser überhaupt trifft
    public LayerMask reflectionLayers;     // LayerMask: an diesen Layern reflektiert der Laser

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawLaser();

    }

    void DrawLaser()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;

        points.Add(start);

        for (int i = 0; i < maxReflections; i++)
        {
            if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance, collisionLayers))
            {
                // Trefferpunkt hinzufügen
                points.Add(hit.point);

                // Prüfen ob der getroffene Layer in reflectionLayers enthalten ist
                if (((1 << hit.collider.gameObject.layer) & reflectionLayers) != 0)
                {
                    // Reflexion
                    direction = Vector3.Reflect(direction, hit.normal);
                    start = hit.point;
                    continue;
                }
                else
                {
                    // Kein Spiegel-Layer -> Laser endet am Objekt
                    break;
                }
            }
            else
            {
                // Kein Treffer -> Linie bis maxDist
                points.Add(start + direction * maxDistance);
                break;
            }
        }

        if (points.Count == 10) {
            Debug.Log("lösung");
        }

        // Punkte an den LineRenderer geben
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        //Debug.Log(points.Count);
    }
}
