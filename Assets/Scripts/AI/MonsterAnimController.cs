using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleSystem;

public class MonsterAnimController : MonoBehaviour
{
    private EnemyAI enemy;
    private Animator animator;

    private void Start()
    {
        enemy = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("isMoving", enemy.isWalking);
        animator.SetBool("isRunning", enemy.isRunning);
        animator.SetBool("turning", enemy.isTurning);
    }


    private void OnEnable()
    {
        BattleSystem.OnBattleAction += HandleBattleAction;
    }

    private void OnDisable()
    {
        BattleSystem.OnBattleAction -= HandleBattleAction;
    }


    private void HandleBattleAction(BattleActionType actionType, int playerNumber)
    {

    }

}
