using MoreMountains.Feedbacks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float wanderRadius = 10.0f;  // Radius within which to stay
    public MMFeedbacks alertFeedback;

    private Transform player;
    private Vector3 directionToPlayer;
    private Vector3 homePosition;  // The center of the wander area
    private Coroutine wanderCoroutine;
    private bool canChase = false;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;

    private void Start()
    {
        player = FindObjectOfType<PlayableCharacter>().transform;
        homePosition = transform.position;  // Set the home position to the initial position
    }

    private void Update()
    {
        UpdateChaseCondition();
        UpdateDirectionToPlayer();
        HandleWandering();
        HandleRotation();
        HandleWalking();
        // If the enemy is not chasing the player and not already wandering, start wandering
        if (!canChase && wanderCoroutine == null)
        {
            wanderCoroutine = StartCoroutine(Wander());
        }
        // If the enemy can chase the player, start the HandleChasing coroutine
        if (canChase)
        {
            StartCoroutine(HandleChasing());
        }
    }


    private void UpdateChaseCondition()
    {
        // Check distance between player and enemies spawn
        canChase = Vector3.Distance(player.position, homePosition) < wanderRadius;
    }

    private void UpdateDirectionToPlayer()
    {
        directionToPlayer = (player.position - transform.position).normalized;
    }

    private void HandleWandering()
    {
        if (wanderCoroutine == null)
        {
            // If the player is within the field of vision
            wanderCoroutine = StartCoroutine(Wander());
        }
    }

    private void HandleRotation()
    {
        if (isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotationSpeed);
        }

        if (isRotatingLeft)
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotationSpeed);
        }
    }

    private void HandleWalking()
    {
        if (isWalking)
        {
            // If outside the wander radius, rotate back towards home
            if (Vector3.Distance(transform.position, homePosition) >= wanderRadius)
            {
                Vector3 directionHome = (homePosition - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionHome), rotationSpeed * Time.deltaTime);
            }

            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private IEnumerator HandleChasing()
    {
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            // Cast a ray towards the player
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, playerLayer))
            {
                // If the player is detected, start chasing
                if (hit.collider.transform == player && canChase)
                {

                    alertFeedback.PlayFeedbacks();
                    // Pause for a second
                    yield return new WaitForSeconds(1f);

                    if (wanderCoroutine != null)
                    {
                        StopCoroutine(wanderCoroutine);
                        wanderCoroutine = null;
                    }
                    // Stop rotating due to wandering
                    isRotatingRight = false;
                    isRotatingLeft = false;
                    ChasePlayer();
                }
            }
        }
    }

    private void ChasePlayer()
    {
        // Calculate the direction and velocity
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        Vector3 velocity = direction * chaseSpeed * Time.deltaTime;

        // Move and face towards the player
        transform.position += velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, chaseSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(1, 4);
        int rotateLorR = Random.Range(1, 2);
        int walkWait = Random.Range(1, 4);
        int walkTime = Random.Range(1, 2);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);

        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }

        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }

        isWandering = false;
        wanderCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        // Draw the field of view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, viewAngleA * viewDistance);
        Gizmos.DrawRay(transform.position, viewAngleB * viewDistance);

        Gizmos.DrawWireSphere(homePosition, wanderRadius);
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
