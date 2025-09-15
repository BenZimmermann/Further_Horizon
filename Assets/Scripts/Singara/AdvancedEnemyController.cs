using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AdvancedEnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;   // feste Punkte im Inspector zuweisen
    public float waitTime = 2f;        // Pause an jedem Punkt

    [Header("Combat Settings")]
    public int EnemyDamage;

    [Header("Vision Settings")]
    public float viewDistance; // maximale Sichtweite
    public float viewAngle;     // Sichtwinkel in Grad
    public float sphereRadius;  // Radius für den SphereCast
    public LayerMask visionMask = ~0;  // standardmäßig Everything

    [Header("Follow Settings")]
    public float loseSightDistance = 20f;   // maximale Verfolgungsreichweite in Metern

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
                    currentState = State.Chase;
                    Debug.Log("Spieler gesehen → Chase");
                }
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        if (isWaiting || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        targetPoint = (targetPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[targetPoint].position);

        isWaiting = false;
    }

    private void Chase()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

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

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 toPlayer = player.position - origin;
        Vector3 dirToPlayer = toPlayer.normalized;

        // SphereCast in Richtung Spieler
        if (Physics.SphereCast(origin, sphereRadius, dirToPlayer, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Sichtwinkel prüfen
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
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.Instance.TakeDamage(EnemyDamage);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 forward = transform.forward;

        // Sichtfeldgrenzen
        Gizmos.color = Color.cyan;
        Quaternion leftRot = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up);
        Gizmos.DrawRay(origin, leftRot * forward * viewDistance);
        Gizmos.DrawRay(origin, rightRot * forward * viewDistance);

        // SphereCast Start
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, sphereRadius);

        // Linie zum Spieler
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, player.position);
    }
}
