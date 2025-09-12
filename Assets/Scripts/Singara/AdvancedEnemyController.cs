using UnityEngine;
using System.Collections;

public class AdvancedEnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float waitTime = 2f;
    public float rotationSpeed = 5f;

    [Header("Combat Settings")]
    public int EnemyDamage;

    [Header("Vision Settings")]
    public float viewDistance = 10f;   // Sichtweite
    public float viewAngle = 45f;      // halber Sichtwinkel
    public float sphereRadius = 0.5f;  // SphereCast-Radius
    public LayerMask visionMask;       // z. B. nur "Player"

    [Header("Follow Settings")]
    public float followRange = 15f;    // maximale Verfolgungsreichweite

    private int targetPoint;
    private bool isWaiting = false;

    private Transform player;
    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    void Start()
    {
        targetPoint = 0;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                CheckVision();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    // -------------------
    // PATROL LOGIK
    // -------------------
    private void Patrol()
    {
        if (isWaiting) return;

        Vector3 targetPos = patrolPoints[targetPoint].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        RotateTowards(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        int nextPoint = (targetPoint + 1) % patrolPoints.Length;

        float timer = 0f;
        while (timer < waitTime)
        {
            RotateTowards(patrolPoints[nextPoint].position);
            timer += Time.deltaTime;
            yield return null;
        }

        targetPoint = nextPoint;
        isWaiting = false;
    }

    // -------------------
    // CHASE LOGIK
    // -------------------
    private void Chase()
    {
        if (player == null) return;

        Vector3 targetPos = player.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        RotateTowards(targetPos);

        // Spieler außer Follow Range? → zurück zu Patrol
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > followRange)
        {
            Debug.Log("Spieler außer Reichweite, zurück zu Patrol.");
            currentState = State.Patrol;
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // -------------------
    // SICHT PRÜFUNG
    // -------------------
    private void CheckVision()
    {
        if (player == null) return;

        Vector3 origin = transform.position + Vector3.up * 1.8f;
        Vector3 forward = transform.forward;

        // SphereCast nach vorne
        if (Physics.SphereCast(origin, sphereRadius, forward, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Richtung zum Spieler
                Vector3 toPlayer = (player.position - origin).normalized;

                // Skalarprodukt (cos(θ))
                float dot = Vector3.Dot(forward, toPlayer);

                // innerhalb des Sichtwinkels?
                if (dot >= Mathf.Cos(viewAngle * Mathf.Deg2Rad))
                {
                    OnPlayerSeen();
                }
            }
        }
    }

    private void OnPlayerSeen()
    {
        Debug.Log("Spieler entdeckt → Verfolgung gestartet!");
        currentState = State.Chase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.Instance.TakeDamage(EnemyDamage);
        }
    }
    // -------------------
    // DEBUG-GIZMOS
    // -------------------
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 1.8f;
        Vector3 forward = transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, forward * viewDistance);

        // Sichtfeldgrenzen
        Quaternion leftRot = Quaternion.AngleAxis(-viewAngle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(viewAngle, Vector3.up);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, leftRot * forward * viewDistance);
        Gizmos.DrawRay(origin, rightRot * forward * viewDistance);

        // SphereCast Endpunkt
        Vector3 endPoint = origin + forward * viewDistance;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(endPoint, sphereRadius);
    }
}
