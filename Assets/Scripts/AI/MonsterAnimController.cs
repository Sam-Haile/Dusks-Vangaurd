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
        BattleSystem.OnEnemyAction += HandleEnemyAction;
    }

    private void OnDisable()
    {
        BattleSystem.OnEnemyAction -= HandleEnemyAction;
    }


    private void HandleEnemyAction(BattleActionType actionType, Unit enemy)
    {
        switch (actionType)
        {
            case BattleActionType.Attack:
                enemy.GetComponent<Animator>().SetTrigger("attack");
                break;
            case BattleActionType.Gaurd:
                enemy.GetComponent<Animator>().SetTrigger("gaurd");
                break;
            case BattleActionType.Arcane:
                enemy.GetComponent<Animator>().SetTrigger("arcane");
                break;
            case BattleActionType.Die:
                float num = ReturnRandomFloat();
                enemy.GetComponent<Animator>().SetTrigger("die");
                enemy.GetComponent<Animator>().SetFloat("deathAnim", num);
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
