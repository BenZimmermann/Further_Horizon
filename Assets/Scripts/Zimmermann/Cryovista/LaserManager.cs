using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    public int maxReflections = 8;        //max reflections
    public float maxDistance = 10f;        //max distance of ray
    public LayerMask collisionLayers;      //all layers that are active
    public LayerMask reflectionLayers;     //LayerMask: the layer witch will reflect the ray (mirror)

    [SerializeField] public GameObject ice;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawLaser();

    }
    /// <summary>
    /// insted of checking every object if it has a hit we count the current bouces of the ray to determent if the player solved the riddle
    /// </summary>
    void DrawLaser()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;

        points.Add(start);

        //for every "bounce" of the ray. until max reflections is reached
        for (int i = 0; i < maxReflections; i++)
        {
            //using raycast as ray to detect the corrct layermask
            if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance, collisionLayers))
            {
                // add hitpoint
                points.Add(hit.point);

                // check if the current layer a revlect layer is
                if (((1 << hit.collider.gameObject.layer) & reflectionLayers) != 0)
                {
                    // reflection with the current object normal 
                    
                    direction = Vector3.Reflect(direction, hit.normal);
                    start = hit.point;
                    continue;
                }
                else
                {
                    // no mirror layer = no reflection
                    break;
                }
            }
            else
            {
                // no hit = ray until max distance
                points.Add(start + direction * maxDistance);
                break;
            }
        }
        if (points.Count == 8) {
            Debug.Log("lösung");
            StartCoroutine(LaserDestroy());
            //reward when solving the riddle
        }

        // fill the points array
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
   private IEnumerator LaserDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(ice);
    }
}
