using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed = 2f;
    public int EnemyDamage = 1;
    public float EnemyWaitTime = 2f;
    public float rotationSpeed = 5f;

    private bool isWaiting = false;

    void Start()
    {
        targetPoint = 0;
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveAndRotate();
        }
    }

    private void MoveAndRotate()
    {
        Vector3 targetPos = patrolPoints[targetPoint].position;

        // Bewegung
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Rotation in Bewegungsrichtung
        RotateTowards(targetPos);

        // Punkt erreicht?
        if (transform.position == targetPos)
        {
            StartCoroutine(EnemyWait());
        }
    }

    private IEnumerator EnemyWait()
    {
        isWaiting = true;

        // Nächsten Punkt schon berechnen
        int nextPoint = targetPoint + 1;
        if (nextPoint >= patrolPoints.Length) nextPoint = 0;

        float timer = 0f;
        while (timer < EnemyWaitTime)
        {
            // Während Wartezeit Richtung des nächsten Ziels drehen
            RotateTowards(patrolPoints[nextPoint].position);

            timer += Time.deltaTime;
            yield return null;
        }

        // Wenn fertig: weiter zum nächsten Ziel
        incrementTargetPoint();
        isWaiting = false;
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f; // keine Neigung nach oben/unten

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

    public void incrementTargetPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.Instance.TakeDamage(EnemyDamage);
            Debug.Log("Player hit by enemy, damage applied: " + EnemyDamage);
        }
    }
}
