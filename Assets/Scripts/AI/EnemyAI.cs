using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Serialized fields for enemy attributes
    public float minWalkTime = 0;
    public float maxWalkTime = 1f;
    [SerializeField] private float viewAngle;
     public float viewDistance;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float chaseSpeed;
     public float speed;
     public float rotationSpeed = 200.0f;
    [SerializeField] private float wanderRadius = 10.0f;  // Radius within which to stay

    // Private variables for enemy behavior
    private Transform player;
    private Vector3 directionToPlayer;
    private Vector3 homePosition;
    private Coroutine wanderCoroutine;
    private Coroutine chaseCoroutine;

    // Flags for enemy states
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    [HideInInspector] public bool isTurning;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isWalking;

    public BoxCollider boxCollider;
    public bool isDefeated = false;
    public string enemyID; // Unique ID for each enemy

    private void Awake()
    {
        // Find the player in the scene
        player = PlayerCollision.instance.transform;
    }

    private void Start()
    {
        //refToSpawner.canSpawn = false;
        // Initialize the home position
        homePosition = transform.position;
        enemyID = gameObject.name + "_" + Guid.NewGuid().ToString();
        //GameData.enemies.Add(this.gameObject);
    }

    private void Update()
    {
        // Update enemy behavior every frame
        UpdateDirectionToPlayer();
        HandleWandering();
        HandleRotation();
        HandleWalking();

        // If the enemy is not chasing the player and not already wandering, start wandering
        if (wanderCoroutine == null)
        {
            isRunning = false;
            wanderCoroutine = StartCoroutine(Wander());
        }
        // If the enemy can chase the player, start the HandleChasing coroutine
        if (chaseCoroutine ==null)
        {
            chaseCoroutine = StartCoroutine(HandleChasing());
        }
    }


    #region Wander Code
    private void HandleWandering()
    {
        if (wanderCoroutine == null && !isRunning)
        {
            // If the player is within the field of vision
            wanderCoroutine = StartCoroutine(Wander());
        }
    }

    /// <summary>
    /// Handles enemy walking behavior.
    /// </summary>
    private void HandleWalking()
    {
        // If outside the wander radius, rotate back towards home
        if (Vector3.Distance(transform.position, homePosition) >= wanderRadius)
        {
            Vector3 directionHome = (homePosition - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionHome), rotationSpeed * Time.deltaTime);
        }

        if (isWalking)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Coroutine for wandering behavior.
    /// </summary>
    private IEnumerator Wander()
    {
        isWalking = true;

        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }

        int rotTime = UnityEngine.Random.Range(1, 3);
        int rotateWait = UnityEngine.Random.Range(1, 4);
        int rotateLorR = UnityEngine.Random.Range(1, 2);
        float walkTime = UnityEngine.Random.Range(minWalkTime, maxWalkTime);


        yield return new WaitForSeconds(walkTime);
        isWalking = false;

        yield return new WaitForSeconds(rotateWait);

        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            isTurning = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
            isTurning = false;
        }

        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            isTurning = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
            isTurning = false;
        }
        wanderCoroutine = null;

    }


    #endregion

    #region Chase Code


    /// <summary>
    /// Coroutine for chasing the player.
    /// </summary>
    private IEnumerator HandleChasing()
    {
        while (true) // keep checking
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                // Cast a ray towards the player
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, playerLayer))
                {
                    // If the player is detected, start chasing
                    if (hit.collider.transform == player)
                    {
                        isRunning = true;
                        isTurning = false;

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
                    else
                        isRunning = false;
                }
            }

            yield return null;
            //yield return new WaitForSeconds(0.5f); // check every half a second
        }
    }


    /// <summary>
    /// Handles the logic for the enemy chasing the player.
    /// </summary>
    private void ChasePlayer()
    {
        // Normalize the direction to get a unit vector
        Vector3 direction = (player.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.position += direction * chaseSpeed * Time.deltaTime;

        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        // Make the enemy face the player
        Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    }

    #endregion

    #region Rotation Code
    /// <summary>
    /// Updates the direction towards the player.
    /// </summary>
    private void UpdateDirectionToPlayer()
    {
        directionToPlayer = (player.position - transform.position).normalized;
    }

    /// <summary>
    /// Handles enemy rotation based on flags.
    /// </summary>
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


    /// <summary>
    /// Converts an angle to a direction vector.
    /// </summary>
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    #endregion


    private void OnDisable()
    {
        // Ensure all coroutines are stopped when the script is disabled
        StopAllCoroutines();
    }

#if UNITY_EDITOR 
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
#endif
}
