using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyPathfindingSystem : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform player;
    public float sightRange = 20f;
    public float viewAngle = 70f;
    public float loseSightTime = 2f;

    private int currentIndex = 0;
    private NavMeshAgent agent;
    private float lostTimer = 0f;
    private Vector3 playerVector;

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;
    private RaycastHit hit;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = waypoints[currentIndex].position;
    }

    void Update()
    {
        playerVector = player.position;
        playerVector.y = transform.position.y;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                DetectPlayer();
                break;

            case State.Chase:
                Chase();
                LosePlayerCheck();
                break;
        }
    }

    void Patrol()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(waypoints[currentIndex].position);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentIndex].position;
        }
    }

    void DetectPlayer()
    {
        Vector3 dirToPlayer = (playerVector - transform.position).normalized;
        //Debug.Log($"Direction to player: {dirToPlayer}");

        float distance = Vector3.Distance(transform.position, playerVector);
        Debug.DrawRay(transform.position, dirToPlayer * distance, Color.green);


        if (distance <= sightRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle <= viewAngle / 2f)
            {
                if (Physics.Raycast(transform.position, dirToPlayer, out hit, distance))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        Debug.Log("Player detected! Switching to Chase state.");
                        currentState = State.Chase;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            Destroy(gameObject);    
        }
    }

    void Chase()
        {
            agent.destination = player.position;

        }

        void LosePlayerCheck()
        {
            Vector3 dirToPlayer = (playerVector - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, playerVector);
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            Debug.DrawRay(transform.position, dirToPlayer * distance, Color.red);

            if (distance > sightRange || angle > viewAngle / 2f || Physics.Raycast(transform.position, dirToPlayer, distance))
            {
                lostTimer += Time.deltaTime;
                if (lostTimer >= loseSightTime)
                {
                    lostTimer = 0f;
                    currentState = State.Patrol;
                    //agent.destination = waypoints[currentIndex].position;
                }
            }
            else
            {
                lostTimer = 0f;
            }
        }
    }
