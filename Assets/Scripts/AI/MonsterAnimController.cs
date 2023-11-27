using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using static BattleSystem;

public class MonsterAnimController : MonoBehaviour
{
    private EnemyAI enemy;
    private Animator animator;

    private float num;

    private BattleSystem battleSystem;

    private float animControllerNum;
    private AnimController animController;

    private Animator dmgAnimator;
    private TextMeshPro dmgText;

    private void Start()
    {
        enemy = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        animController = FindObjectOfType<AnimController>();

        battleSystem = FindAnyObjectByType<BattleSystem>();
    }

    void Update()
    {
        animator.SetBool("isWalking", enemy.isWalking);
        animator.SetBool("isRunning", enemy.isRunning);
        animator.SetBool("turning", enemy.isTurning);
    }

    private void OnEnable()
    {
        BattleSystem.OnBattleAction += HandleEnemyAction;
    }

    private void OnDisable()
    {
        BattleSystem.OnBattleAction -= HandleEnemyAction;
    }

    private void HandleEnemyAction(BattleActionType actionType, Unit enemy)
    {
        dmgAnimator = enemy.damageNumbers.GetComponent<Animator>();
        dmgText = enemy.damageNumbers.GetComponent<TextMeshPro>();

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
            case BattleActionType.Damaged:
                StartCoroutine(WaitForReaction(1f, 0f, "damaged", enemy.GetComponent<Animator>()));

                StartCoroutine(DamageEnemy(2, enemy));

                //animControllerNum = animController.GetNumValue();
                //if (animControllerNum < .33)

                //else if (animControllerNum > .33 && animControllerNum < .66)
                //{
                //    StartCoroutine(WaitForReaction(1f, 0f, "damaged", enemy));
                //    StartCoroutine(WaitForReaction(.5f, 0f, "damaged", enemy));
                //}
                //else
                //{
                //    StartCoroutine(WaitForReaction(1f, 0f, "damaged", enemy));
                //    StartCoroutine(WaitForReaction(.5f, 0f, "damaged", enemy));
                //    StartCoroutine(WaitForReaction(.5f, 0f, "damaged", enemy));
                //}
                break;
            default:
                break;
        }
    }


    private float ReturnRandomFloat()
    {
        return Random.Range(0.0f, 1.0f);
    }

    IEnumerator WaitForReaction(float initialWait, float finalWaitTime, string trigger, Animator animator)
    {
        yield return new WaitForSeconds(initialWait);
        animator.SetTrigger(trigger);
    }

    /// <summary>
    /// Displays the damage texts and turns it off after X seconds
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator DamageEnemy(float waitTime, Unit enemy)
    {
        enemy.damageNumbers.SetActive(true);
        dmgText.text = battleSystem?.LastDamage.ToString();
        dmgAnimator.SetTrigger("damaged");
        yield return new WaitForSeconds(waitTime);
        enemy.damageNumbers.SetActive(false);
    }

}
