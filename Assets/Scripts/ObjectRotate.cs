using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour
{

    private Vector2 turn;
    public float sensitivity = .5f;
    public Vector3 deltaMove;
    public float speed = 1f;

    // Define min and max for rotation
    public float maxXRotation = 20.0f;
    public float minXRotation = -20.0f;
    public float maxYRotation = 20.0f;
    public float minYRotation = -20.0f;


    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
        {
            // Calculate the rotation based on mouse movement
            float xRotation = -Input.GetAxisRaw("Mouse X") * sensitivity;
            float yRotation = -Input.GetAxisRaw("Mouse Y") * sensitivity;

            // Rotate object within min and max
            turn.x = Mathf.Clamp(turn.x + xRotation, minXRotation, maxXRotation);
            turn.y = Mathf.Clamp(turn.y + yRotation, minYRotation, maxYRotation);

            // Apply the rotation
            Quaternion targetRotation = Quaternion.Euler(turn.y + 57, turn.x + 225, 0);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            Debug.Log("Going Back");
            Quaternion targetRotation = Quaternion.Euler(57, 225, 0);
            float rotationSpeed = 1f; // Adjust this value to change the speed of rotation
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        deltaMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);
        this.transform.Translate(deltaMove);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawRay(this.transform.position, this.transform.up * 10);
    }

}

