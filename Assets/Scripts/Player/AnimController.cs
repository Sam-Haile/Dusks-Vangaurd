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


    public GameObject swordBack;
    public GameObject swordHand;

    private void Awake()
    {
        playerAnimator = player1.GetComponent<Animator>();
        player2Animatior = player2.GetComponent<Animator>();
    }

    private void Start()
    {
        playerMovement = player1.GetComponent<PlayerMovement>();
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


    private void OnLevelWasLoaded(int level)
    {
        // If it's a battle, swtich the animation controllers
        if(level == 2)
        {
            swordBack.SetActive(false);
            swordHand.SetActive(true);
            playerAnimator.SetTrigger("battle");
        }
        else if(level == 1){
            playerAnimator.SetTrigger("battleEnd");
            swordBack.SetActive(true);
            swordHand.SetActive(false);
        }
    }


    private void OnEnable()
    {
        BattleSystem.OnBattleAction += HandleBattleAction;
    }

    private void OnDisable()
    {
        BattleSystem.OnBattleAction -= HandleBattleAction;
    }

    private void HandleBattleAction()
    {
        // Do something, like triggering a different animation
        Debug.Log("Attacking Animation");
        playerAnimator.SetTrigger("attack");
    }

}
