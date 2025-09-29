using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AdvancedEnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;   // Array of patrol points (set in Inspector)
    public float waitTime = 2f;        // Wait time at each patrol point

    [Header("Combat Settings")]
    public int EnemyDamage;

    [Header("Vision Settings")]
    public float viewDistance;         // Maximum vision distance
    public float viewAngle;            // Vision cone angle in degrees
    public float sphereRadius;         // Radius for SphereCast (vision detection)
    public LayerMask visionMask = ~0;  // Layers considered for vision (default = Everything)

    [Header("Follow Settings")]
    public float loseSightDistance = 20f;  

    private int targetPoint = 0;
    private bool isWaiting = false;

    private Transform player;
    private NavMeshAgent agent;

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Kein NavMeshAgent auf dem Gegner gefunden!");
            enabled = false;
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[targetPoint].position);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (CheckVision())
                {
                    // Switch to chase if player is detected
                    currentState = State.Chase;
                    Debug.Log("Spieler gesehen Chase");
                }
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        // Do nothing if waiting or no patrol points
        if (isWaiting || patrolPoints.Length == 0) return;

        // When enemy reaches patrol point = wait before moving on
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        // Enemy pauses at patrol point
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // Move to next patrol point (loops around)
        targetPoint = (targetPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[targetPoint].position);

        isWaiting = false;
    }

    private void Chase()
    {
        if (player == null) return;

        // Continuously move toward player
        agent.SetDestination(player.position);

        // Check if player is too far -> stop chasing
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > loseSightDistance)
        {
            Debug.Log("Spieler außer Reichweite → zurück zu Patrol");
            currentState = State.Patrol;
            agent.SetDestination(patrolPoints[targetPoint].position);
        }
    }

    private bool CheckVision()
    {
        if (player == null) return false;

        // Eye-level origin for vision check
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 toPlayer = player.position - origin;
        Vector3 dirToPlayer = toPlayer.normalized;

        // Cast a sphere-shaped ray toward the player
        if (Physics.SphereCast(origin, sphereRadius, dirToPlayer, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Check if player is within vision cone (dot product)
                float dot = Vector3.Dot(transform.forward, dirToPlayer);

                if (dot >= Mathf.Cos(viewAngle * Mathf.Deg2Rad * 0.5f))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deal damage if colliding with player
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.Instance.TakeDamage(EnemyDamage);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Draw vision cone boundaries
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 forward = transform.forward;

        Gizmos.color = Color.cyan;
        Quaternion leftRot = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up);
        Gizmos.DrawRay(origin, leftRot * forward * viewDistance);
        Gizmos.DrawRay(origin, rightRot * forward * viewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, sphereRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, player.position);
    }
}
