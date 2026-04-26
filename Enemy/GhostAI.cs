using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GhostAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Investigate,
        Chase,
        Search,
        Cooldown
    }

    public State currentState;

    [Header("References")]
    public Transform[] patrolPoints;
    public LayerMask obstacleMask;
    public Transform eyePoint;

    [Header("Ranges")]
    public float hearRadius = 12f;
    public float sightRange = 15f;
    public float sightAngle = 70f;
    public float attackRange = 1.8f;

    [Header("Timers")]
    public float searchTime = 8f;
    public float cooldownTime = 5f;

    private NavMeshAgent agent;
    private Transform player;
    private int patrolIndex;
    private float timer;
    private Vector3 lastKnownPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentState = State.Patrol;
        GoNextPatrol();
    }

    void Update()
    {
        switch(currentState)
        {
            case State.Patrol:
                PatrolUpdate();
                break;

            case State.Investigate:
                InvestigateUpdate();
                break;

            case State.Chase:
                ChaseUpdate();
                break;

            case State.Search:
                SearchUpdate();
                break;

            case State.Cooldown:
                CooldownUpdate();
                break;
        }

        CheckVision();
        CheckLightFear();
        CheckAttack();
    }

    #region STATES

    void PatrolUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
            GoNextPatrol();
    }

    void InvestigateUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            currentState = State.Search;
            timer = searchTime;
        }
    }

    void ChaseUpdate()
    {
        agent.speed = 5.5f;
        agent.SetDestination(player.position);

        lastKnownPos = player.position;

        if (!CanSeePlayer())
        {
            currentState = State.Search;
            timer = searchTime;
            agent.SetDestination(lastKnownPos);
        }
    }

    void SearchUpdate()
    {
        timer -= Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            Vector3 randomPoint = lastKnownPos + Random.insideUnitSphere * 6f;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 6f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }

        if (timer <= 0)
        {
            currentState = State.Cooldown;
            timer = cooldownTime;
        }
    }

    void CooldownUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            currentState = State.Patrol;
            agent.speed = 3.5f;
            GoNextPatrol();
        }
    }

    #endregion

    #region SENSING

    public void HearSound(Vector3 pos)
    {
        float dist = Vector3.Distance(transform.position, pos);

        if (dist <= hearRadius)
        {
            currentState = State.Investigate;
            agent.SetDestination(pos);
        }
    }

    void CheckVision()
    {
        if (CanSeePlayer())
        {
            currentState = State.Chase;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - eyePoint.position).normalized;
        float dist = Vector3.Distance(eyePoint.position, player.position);

        if (dist > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > sightAngle * 0.5f) return false;

        if (Physics.Raycast(eyePoint.position, dir, out RaycastHit hit, sightRange))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }

        return false;
    }

    #endregion

    #region ATTACK

    void CheckAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && currentState == State.Chase)
        {
            JumpscareManager.Instance.StartJumpscare(transform);
        }
    }

    IEnumerator JumpscareKill()
    {
        agent.isStopped = true;

        Debug.Log("JUMPSCARE!");

        yield return new WaitForSeconds(2f);

        Debug.Log("GAME OVER");
    }

    #endregion

    #region LIGHT FEAR

    void CheckLightFear()
    {
        Light flash = FindObjectOfType<Light>();

        if (flash == null || !flash.enabled) return;

        float dist = Vector3.Distance(transform.position, flash.transform.position);

        if (dist < 7f && currentState != State.Chase)
        {
            Vector3 dir = (transform.position - flash.transform.position).normalized;
            transform.position += dir * 2f * Time.deltaTime;
        }
    }

    #endregion

    void GoNextPatrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
}