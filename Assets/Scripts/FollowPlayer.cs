using Cinemachine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FollowPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private CharacterController charController;
    public float p2moveSpeed = 1f;
    public float rotateSpeed = 8f;
    
    public bool canWalk = true;
    private Coroutine walkCoroutine;
    public Animator playerAnimator;

    private bool moving = false;

    private Vector3 currentPosition;
    private float distanceToPlayer;
    private Vector3 direction;
    private Vector3 velocity;
    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        // Only start the Walk coroutine if it's not already running and if canWalk is true
        if (walkCoroutine == null && canWalk)
            walkCoroutine = StartCoroutine(Walk());

        if (playerMovement.isSprinting)
        {
            p2moveSpeed = playerMovement.moveSpeed * playerMovement.sprintMultiplier;
            playerAnimator.SetBool("isSprinting", true);
        }
        else
        {
            p2moveSpeed = playerMovement.moveSpeed;
            playerAnimator.SetBool("isSprinting", false);
        }

        if (playerMovement.isMoving)
            playerAnimator.SetBool("isMoving", true);
        else
            playerAnimator.SetBool("isMoving", false);


    }

    IEnumerator Walk()
    {
        while (canWalk)
        {
            moving = false;

            currentPosition = charController.transform.position;

            // Proximity check to the player
            distanceToPlayer = Vector3.Distance(currentPosition, playerMovement.gameObject.transform.position);

            // If the NPC is very close to the player, simply wait
            if (distanceToPlayer < 1.5f)
            {
                yield return null;
                continue;
            }

            // Check if we're too far from the player
            if (distanceToPlayer > 0.5f)
            {
                moving = true;
                // If we're too far, aim directly for the player
                direction = playerMovement.gameObject.transform.position - currentPosition;
                direction.Normalize();
                velocity = direction * p2moveSpeed * Time.deltaTime;

                // Move and face towards the player
                charController.Move(velocity);
                if (direction != Vector3.zero)
                {
                    direction.y = 0;
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    Quaternion newRotation = Quaternion.Lerp(charController.transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
                    charController.transform.rotation = newRotation;
                }
            }
            else if (playerMovement.breadcrumbs.Count > 0)
            {
                moving = true;
                // If we're close enough, just follow the breadcrumbs
                Vector3 targetPosition = playerMovement.breadcrumbs.Peek();
                Vector3 direction = targetPosition - currentPosition;

                direction.Normalize();
                Vector3 velocity = direction * p2moveSpeed * Time.deltaTime;

                // Move and face towards the next breadcrumb
                currentPosition += velocity;
                charController.Move(velocity);

                if (Vector3.Distance(currentPosition, targetPosition) <= 0.5f)
                {
                    playerMovement.breadcrumbs.Dequeue();
                }

                if (direction != Vector3.zero)
                {
                    direction.y = 0;
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    Quaternion newRotation = Quaternion.Lerp(charController.transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
                    charController.transform.rotation = newRotation;
                }
            }

            yield return null;
        }
    }
}
