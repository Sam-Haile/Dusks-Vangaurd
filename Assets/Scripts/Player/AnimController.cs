using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Animator player2Animatior;

    private float timeIdle = 0;
    private bool playIdleAnim = false;
    
    private void Start()
    {
        playerMovement = player1.GetComponent<PlayerMovement>();
        playerAnimator = player1.GetComponent<Animator>();
        player2Animatior = player2.GetComponent<Animator>();
    }

    private void Update()
    {

        if (playerMovement.isMoving)
        {
            SetBool("isMoving", true);
            playerAnimator.SetBool("lookAround", true);
            timeIdle = 0;
        }
        else
        {
            SetBool("isMoving", false);
            //timeIdle += Time.deltaTime;

            //if(timeIdle >= 7)
            //{
            //    //playerAnimator.SetTrigger("lookAround");
            //    timeIdle = 0;
            //}
        }

        if (playerMovement.isSprinting)
            SetBool("isSprinting", true);
        else
            SetBool("isSprinting", false);


    }

    public void SetBool(string tag, bool flag)
    {
        playerAnimator.SetBool(tag, flag);
        player2Animatior.SetBool(tag, flag);
    }





}
