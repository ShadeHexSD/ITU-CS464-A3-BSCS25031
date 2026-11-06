using UnityEngine;

// Lecture 07's patrol modes, expanded to the assignment's 3 states.
public enum PatrolMode { Loop, PingPong, Random }
public enum EnemyState { Patrol, Chase, Return }

[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] float speed = 2f;
    [SerializeField] float waitTime = 0f;
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] float arriveDistance = 0.2f;

    [Header("Chase and return")]
    [SerializeField] float chaseSpeed = 3.5f;
    [SerializeField] float chaseRange = 4f;
    [SerializeField] float giveUpRange = 6f;

    Transform player;
    EnemyState state = EnemyState.Patrol;
    int index, step = 1;
    float waitUntil;

    void Awake()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    void Start()
    {
        PlayerController found = FindAnyObjectByType<PlayerController>();
        if (found != null) player = found.transform;
        if (giveUpRange <= chaseRange) giveUpRange = chaseRange + 2f;
    }

    void Update()
    {
        if (points == null || points.Length < 3 || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (state != EnemyState.Chase && distance < chaseRange)
            state = EnemyState.Chase;
        else if (state == EnemyState.Chase && distance > giveUpRange)
            state = EnemyState.Return;

        if (state == EnemyState.Chase)
        {
            MoveTo(player.position, chaseSpeed);
        }
        else if (state == EnemyState.Return)
        {
            MoveTo(points[index].position, speed);
            if (Arrived(points[index].position)) state = EnemyState.Patrol;
        }
        else if (Time.time >= waitUntil)
        {
            MoveTo(points[index].position, speed);
            if (Arrived(points[index].position))
            {
                index = NextIndex();
                waitUntil = Time.time + waitTime;
            }
        }
    }

    bool Arrived(Vector3 target)
    {
        // Assignment 03: distance tolerance, not position == target.
        target.y = transform.position.y;
        return Vector3.Distance(transform.position, target) < arriveDistance;
    }

    void MoveTo(Vector3 target, float moveSpeed)
    {
        // Keep the guard on its own height; use open patrol routes (no wall pathfinding).
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position, target, moveSpeed * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop) return (index + 1) % points.Length;
        if (mode == PatrolMode.PingPong)
        {
            if (index + step < 0 || index + step >= points.Length) step = -step;
            return index + step;
        }
        int next = Random.Range(0, points.Length);
        return next == index ? (next + 1) % points.Length : next;
    }
}
