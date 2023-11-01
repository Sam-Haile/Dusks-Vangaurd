using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleSystem;

public class MonsterAnimController : MonoBehaviour
{
    private EnemyAI enemy;
    private Animator animator;

    private float num;

    private float animControllerNum;
    private AnimController animController;

    
    private void Start()
    {
        enemy = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        animController = FindObjectOfType<AnimController>();
    }

    void Update()
    {
        animator.SetBool("isWalking", enemy.isWalking);
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
            case BattleActionType.Start:
                enemy.GetComponent<Animator>().SetTrigger("battle");
                break;
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
                num = ReturnRandomFloat();
                enemy.GetComponent<Animator>().SetTrigger("die");
                enemy.GetComponent<Animator>().SetFloat("deathAnim", num);
                break;
            case BattleActionType.Hit:
                animControllerNum = animController.GetNumValue();
                if (animControllerNum < .33)
                    StartCoroutine(WaitForReaction(1f, 0f, "hit", enemy));
                else if (animControllerNum > .33 && animControllerNum < .66)
                {
                    StartCoroutine(WaitForReaction(1f, 0f, "hit", enemy));
                    StartCoroutine(WaitForReaction(.5f, 0f, "hit", enemy));
                }
                else
                {
                    StartCoroutine(WaitForReaction(1f, 0f, "hit", enemy));
                    StartCoroutine(WaitForReaction(.5f, 0f, "hit", enemy));
                    StartCoroutine(WaitForReaction(.5f, 0f, "hit", enemy));
                }
                break;
            default:
                break;
        }
    }


    private float ReturnRandomFloat()
    {
        return Random.Range(0.0f, 1.0f);
    }

    IEnumerator WaitForReaction(float initialWait, float finalWaitTime, string trigger, Unit enemy)
    {
        yield return new WaitForSeconds(initialWait);
        enemy.GetComponent<Animator>().SetTrigger(trigger);
    }

}
