using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public PlayerMovement player; 
    public float maintainDistance; // The distance to maintain from the player
    private Vector3 targetPosition;
    public float followSpeed = 3f; // Speed of following

    void Update()
    {
        
    }

}
