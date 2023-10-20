using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController character;
    private Vector3 movement;

    [HideInInspector] public float walkSpeed = 2f;
    public float moveSpeed;  // Adjust this to control the player's movement speed
    [HideInInspector] public float sprintMultiplier = 1.5f;  // Adjust this to control the sprint speed multiplier
    private float gravity = 2000f;
    public bool isSprinting = false;
    public bool isMoving;

    public int rotationSpeed;
    [SerializeField]
    public Quaternion targetRotation;
    
    float horizontalMovement;
    float verticalMovement; 
    [SerializeField]
    public Queue<Vector3> breadcrumbs = new Queue<Vector3>();
    private Vector3 destination = Vector3.zero;
    public Vector3 lastBreadcrumbPosition;

    public float num;

    public Animator playerAnimator;

    void Start()
    {
        character = GetComponent<CharacterController>();
        lastBreadcrumbPosition = transform.position;
        breadcrumbs.Enqueue(lastBreadcrumbPosition);
        walkSpeed = 2f;
    }

    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        //Get cameras forward and right vectors
        Vector3 camFwdVec = Camera.main.transform.forward;
        camFwdVec.y = 0;

        Vector3 camRghtVec = Camera.main.transform.right;
        camRghtVec.y = 0;

        //Adjust destination vector based on the camera's forward and right vectors
        destination = camRghtVec * horizontalMovement + camFwdVec * verticalMovement;
        destination.Normalize();
        destination *= moveSpeed;

        if(destination.magnitude > .1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(destination);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        destination.y -= gravity * Time.deltaTime; //Apply gravity

        character.Move(destination * Time.deltaTime); //Move character

        Sprint();
        DropBreadCrumbs();
        HandleAnimations();
       
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            playerAnimator.SetBool("isSprinting", true);
            isSprinting = true;
            moveSpeed = walkSpeed * sprintMultiplier;
        }
        else
        {
            playerAnimator.SetBool("isSprinting", false);
            isSprinting = false;
            moveSpeed = walkSpeed;
        }
    }

    private void DropBreadCrumbs()
    {
        float distanceSinceLastBreadcrumb = Vector3.Distance(transform.position, lastBreadcrumbPosition);

        if (distanceSinceLastBreadcrumb > num)
        {
            lastBreadcrumbPosition = transform.position;
            breadcrumbs.Enqueue(transform.position);
        }

        // Limit the queue to a certain size
        while (breadcrumbs.Count > 15)
        {
            breadcrumbs.Dequeue();
        }
    }

    private void HandleAnimations()
    {
        if (destination.x != 0 && destination.z != 0)
        {
            playerAnimator.SetBool("isMoving", true);
            isMoving = true;
        }
        else
        {
            playerAnimator.SetBool("isMoving", false);
            isMoving = false;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var breadcrumb in breadcrumbs)
        {
            Gizmos.DrawCube(breadcrumb, new Vector3(0.2f, 0.2f, 0.2f)); // Adjust the size as needed
        }
    }
}
