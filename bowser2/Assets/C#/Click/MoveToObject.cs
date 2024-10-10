using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MoveToObject : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 startPosition;
    public float stopDistance = 1.0f; // Distancia para detenerse frente al objeto

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;

        // Asegurarse de que el agente está en el NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("El agente no está sobre un NavMesh");
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPositionAndBack(targetPosition));
    }

    private IEnumerator MoveToPositionAndBack(Vector3 targetPosition)
    {
        agent.stoppingDistance = stopDistance; // Establecer distancia de parada
        agent.SetDestination(targetPosition);

        // Esperar hasta que el agente llegue a la distancia de parada
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

       

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
    }
}
