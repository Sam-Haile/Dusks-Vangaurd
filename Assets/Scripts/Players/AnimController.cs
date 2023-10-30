using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static BattleSystem;

public class AnimController : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Animator player2Animator;

    public GameObject swordBack;
    public GameObject swordHand;

    public float num;

    public float GetNumValue()
    {
        return num;
    }

    private void Awake()
    {
        playerAnimator = player1.GetComponent<Animator>();
        player2Animator = player2.GetComponent<Animator>();
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
        player2Animator.SetBool(tag, flag);
    }


    private void OnLevelWasLoaded(int level)
    {
        // If it's a battle, swtich the animation controllers
        if(level == 2)
        {
            swordBack.SetActive(false);
            swordHand.SetActive(true);
            playerAnimator.SetTrigger("battle");
            player2Animator.SetTrigger("battle");
        }
        else if(level == 1){
            playerAnimator.SetTrigger("battleEnd");
            player2Animator.SetTrigger("battleEnd");
            swordBack.SetActive(true);
            swordHand.SetActive(false);
        }
    }


    private void OnEnable()
    {
        BattleSystem.OnPlayerAction += HandlePlayerAction;
    }

    private void OnDisable()
    {
        BattleSystem.OnPlayerAction -= HandlePlayerAction;
    }

    private void HandlePlayerAction(BattleActionType actionType, int playerNumber)
    {
        switch (actionType)
        {
            case BattleActionType.Start:
                
                break;
            case BattleActionType.Attack:
                if(playerNumber == 1)
                {
                    num = .89f;
                    playerAnimator.SetTrigger("attack");
                    playerAnimator.SetFloat("attackAnim", num);
                }
                else if(playerNumber == 2)
                    player2Animator.SetTrigger("attack");
                break;
            case BattleActionType.Gaurd:
                if (playerNumber == 1)
                    playerAnimator.SetTrigger("gaurd");
                else if (playerNumber == 2)
                    player2Animator.SetTrigger("gaurd");
                break;
            case BattleActionType.Arcane:
                if (playerNumber == 1)
                    playerAnimator.SetTrigger("arcane");
                else if (playerNumber == 2)
                    player2Animator.SetTrigger("arcane");
                break;
            case BattleActionType.Run:
                if (playerNumber == 1)
                    playerAnimator.SetTrigger("run");
                else if (playerNumber == 2)
                    player2Animator.SetTrigger("run");
                break;
            case BattleActionType.Die:
                if (playerNumber == 1)
                    playerAnimator.SetTrigger("die");
                else if (playerNumber == 2)
                    player2Animator.SetTrigger("die");
                break;
            default:
                break;
        }
    }


    private float ReturnRandomFloat()
    {
        return Random.Range(0.0f, 1.0f);
    }

}
