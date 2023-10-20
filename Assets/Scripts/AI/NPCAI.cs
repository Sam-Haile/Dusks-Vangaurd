using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NPCAI : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    public List<Transform> npcPath;
    private Transform player;
    private Coroutine talkCoroutine;
    private Coroutine walkCoroutine;
    public bool canWalk = true;
    private int currentPathIndex = 0;

    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private LayerMask playerLayer;
    private Vector3 directionToPlayer;


    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void Update()
    {
        UpdateDirectionToPlayer();

        // Only start the Walk coroutine if it's not already running and if canWalk is true
        if (DetectPlayer() || dialogueTrigger.isTalking)
        {
            canWalk = false;
            if (walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);
                walkCoroutine = null;
            }
        }
        else if (!DetectPlayer() && !dialogueTrigger.isTalking && canWalk && walkCoroutine == null)
        {
            walkCoroutine = StartCoroutine(Walk());
        }
        else if(!DetectPlayer() && !dialogueTrigger.isTalking)
        {
            canWalk = true;
        }
        
    }

    /// <summary>
    /// Creates a ray from the front of the NPC to detect the player
    /// </summary>
    /// <returns> True if player is in front of the NPC</returns>
    private bool DetectPlayer()
    {
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            // Cast a ray towards the player
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, playerLayer))
            {
                // If the player is detected, start chasing
                if (hit.collider.transform == player)
                {

                    // Calculate the direction and velocity
                    Vector3 direction = player.position - transform.position;
                    //direction.Normalize();

                    // Move and face towards the player
                    if (direction != Vector3.zero)
                    {
                        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
                        Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, moveSpeed * Time.deltaTime);
                    }

                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateDirectionToPlayer()
    {
        directionToPlayer = (player.position - transform.position).normalized;
    }

    /// <summary>
    /// NPC's who can be spoken to will stop moving
    /// </summary>
    /// <returns></returns>
    IEnumerator Talk()
    {
        while (DialogueManager.instance.isActive)
        {
            // Calculate the direction and velocity
            Vector3 direction = player.position - transform.position;
            //direction.Normalize();

            // Move and face towards the player
            if (direction != Vector3.zero)
            {
                Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
                Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, moveSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    /// <summary>
    /// NPC's using this coroutine will follow a predetermined path and loop through it
    /// </summary>
    /// <returns></returns>
    IEnumerator Walk()
    {
        while (canWalk) // Infinite Loop
        {
            Vector3 targetPosition = npcPath[currentPathIndex].position;
            while (Vector3.Distance(transform.position, targetPosition) > .5f)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1f,5f));

            currentPathIndex = (currentPathIndex + 1) % npcPath.Count;
        }

    }

    private void OnDrawGizmos()
    {
        foreach (Transform location in npcPath)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(location.position, .3f);

            // Draw the field of view
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);


            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, viewAngleA * viewDistance);
            Gizmos.DrawRay(transform.position, viewAngleB * viewDistance);
        }
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
