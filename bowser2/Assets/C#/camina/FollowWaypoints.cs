using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FollowWaypoints : MonoBehaviour
{
    public Transform[] waypoints; // Array de transformaciones de los puntos de control
    public float speed = 3.5f;    // Velocidad de movimiento

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        if (waypoints.Length > 0 && agent.isOnNavMesh)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            NextWaypoint();
        }
    }

    void NextWaypoint()
    {
        if (currentWaypointIndex < waypoints.Length - 1)
        {
            currentWaypointIndex++;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else
        {
            enabled = false; // Desactiva el script cuando se llega al último waypoint
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == waypoints[currentWaypointIndex])
        {
            NextWaypoint();
        }
    }
    private IEnumerator RotateAfterPath()
    {
        float rotationDuration = 1.0f; // Duración de la rotación
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0); // Rotar 90 grados sobre el eje Y
        float time = 0;

        while (time < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / rotationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation; // Asegura la rotación final
    }
}
