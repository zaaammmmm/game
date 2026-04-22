using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Investigate,
        Chase
    }

    public State currentState;

    public Transform[] patrolPoints;
    public float detectRange = 8f;
    public float chaseRange = 12f;
    public float lightRange = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private int patrolIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentState = State.Patrol;
        GoNextPatrol();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (dist < detectRange)
                    currentState = State.Chase;

                break;

            case State.Investigate:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                    currentState = State.Patrol;

                if (dist < detectRange)
                    currentState = State.Chase;

                break;

            case State.Chase:
                agent.SetDestination(player.position);

                if (dist > chaseRange)
                    currentState = State.Patrol;

                break;
        }

        CheckFlashlight();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
            GoNextPatrol();
    }

    void GoNextPatrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    public void HearSound(Vector3 pos)
    {
        currentState = State.Investigate;
        agent.SetDestination(pos);
    }

    void CheckFlashlight()
    {
        Light flash = FindObjectOfType<Light>();

        if (flash == null || !flash.enabled) return;

        float dist = Vector3.Distance(transform.position, flash.transform.position);

        if (dist < lightRange)
        {
            Vector3 dir = (transform.position - flash.transform.position).normalized;
            transform.position += dir * 3f * Time.deltaTime;
        }
    }
}