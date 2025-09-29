using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Transform[] patrolPoints;    
    public int targetPoint;             // Array of points the enemy will patrol
    public float speed = 2f;            // Current target point index
    public int EnemyDamage = 1;         // Movement speed
    public float EnemyWaitTime = 2f;    // Time to wait at each patrol point
    public float rotationSpeed = 5f;    // Rotation speed to face target

    private bool isWaiting = false;

    void Start()
    {
        // Start patrolling from the first point
        targetPoint = 0;
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveAndRotate();
        }
    }

    /// <summary>
    /// Moves the enemy towards the current target point and rotates towards it.
    /// </summary>
    private void MoveAndRotate()
    {
        Vector3 targetPos = patrolPoints[targetPoint].position;

        // Move towards the target point
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Rotate to face the movement direction
        RotateTowards(targetPos);

        // Check if target point has been reached
        if (transform.position == targetPos)
        {
            StartCoroutine(EnemyWait());
        }
    }
    /// <summary>
    /// Coroutine to wait at a patrol point, and rotate towards the next point while waiting.
    /// </summary>
    private IEnumerator EnemyWait()
    {
        isWaiting = true;

        // Determine the next patrol point
        int nextPoint = targetPoint + 1;
        if (nextPoint >= patrolPoints.Length) nextPoint = 0;

        float timer = 0f;
        while (timer < EnemyWaitTime)
        {
            // Smoothly rotate towards the next patrol point during wait
            RotateTowards(patrolPoints[nextPoint].position);

            timer += Time.deltaTime;
            yield return null;
        }

        // After waiting, increment target point and resume movement
        incrementTargetPoint();
        isWaiting = false;
    }
    /// <summary>
    /// Rotates the enemy to face the target position smoothly.
    /// </summary>
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
    /// <summary>
    /// Advances the target point index to the next patrol point.
    /// Loops back to 0 if at the end of the array.
    /// </summary>
    public void incrementTargetPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }
    /// <summary>
    /// Handles player collision and applies damage.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.Instance.TakeDamage(EnemyDamage);
            Debug.Log("Player hit by enemy, damage applied: " + EnemyDamage);
        }
    }
}
