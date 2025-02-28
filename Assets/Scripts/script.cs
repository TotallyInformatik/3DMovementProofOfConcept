using UnityEngine;
using UnityEngine.AI;

public class script : MonoBehaviour
{
    public Transform target;
    public float speed = 6f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;

    private NavMeshAgent agent;
    private float timer;
    private bool triggered = false;
    private bool hit = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (hit) return; // Don't move if recently hit

        if (triggered)
        {
            // Follow the player using NavMeshAgent
            if (target != null)
            {
                agent.SetDestination(target.position);
            }
        }
        else if (timer >= wanderTimer)
        {
            Vector3 newPos = GetRandomNavMeshPosition();
            if (newPos != transform.position) // Only set if a valid position is found
            {
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            triggered = true;
            if (target == null) target = other.transform; // Assign target if not already
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            triggered = false;
        }
    }

    public void Hit()
    {
        hit = true;
        agent.isStopped = true; // Stop moving
        Invoke(nameof(ResetHit), 1f);
    }

    void ResetHit()
    {
        hit = false;
        agent.isStopped = false; // Resume movement
    }

    Vector3 GetRandomNavMeshPosition()
    {
        for (int i = 0; i < 5; i++) // Try multiple times for a valid point
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return transform.position; // Fallback if no valid position found
    }
}
