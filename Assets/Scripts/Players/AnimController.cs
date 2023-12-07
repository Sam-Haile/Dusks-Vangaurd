using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static BattleSystem;

public class AnimController : MonoBehaviour
{
    public List<PlayableCharacter> partyManager;
    Animator[] animators;


    public GameObject swordBack;
    public GameObject swordHand;

    private Animator dmgAnimator;
    private TextMeshPro dmgText;

    private BattleSystem battleSystem;

    public float num;
    private bool hasBattled = false;

    private void OnEnable()
    {
        OnBattleAction += HandlePlayerAction;
    }

    private void OnDisable()
    {
        OnBattleAction -= HandlePlayerAction;
    }

    public float GetNumValue()
    {
        return num;
    }
    private void Awake()
    {
    }

    private void Start()
    {
        partyManager = PartyManager.instance.partyMembers;
        animators = new Animator[partyManager.Count];

        for (int i = 0; i < partyManager.Count; i++)
        {
            animators[i] = partyManager[i].GetComponent<Animator>();
        }
    }

    // Updates animation state in overworld
    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (PlayerMovement.instance.isMoving)
            SetAnimationBools("isMoving", true);
        else
            SetAnimationBools("isMoving", false);

        if (PlayerMovement.instance.isSprinting)
            SetAnimationBools("isSprinting", true);
        else
            SetAnimationBools("isSprinting", false);
    }


    public void SetAnimationBools(string tag, bool flag)
    {
        foreach (Animator animator in animators)
            animator.SetBool(tag, flag);
    }

    public void SetAnimationTriggers(string tag)
    {
        foreach (Animator animator in animators)
            animator.SetTrigger(tag);
    }

    private void OnLevelWasLoaded(int level)
    {
        // If it's a battle, swtich the animation controllers
        if (level == 2)
        {
            battleSystem = FindAnyObjectByType<BattleSystem>();
            hasBattled = true;
            swordBack.SetActive(false);
            swordHand.SetActive(true);
        }
        else if (level == 1)
        {
            swordBack.SetActive(true);
            swordHand.SetActive(false);

            if (hasBattled)
            {
                SetAnimationTriggers("battleEnd");
            }

            hasBattled = false;
        }
    }

    private void HandlePlayerAction(BattleActionType actionType, Unit player, UnitType u)
    {
        dmgAnimator = player.damageNumbers.GetComponent<Animator>();
        dmgText = player.damageNumbers.GetComponent<TextMeshPro>();

        if (u == UnitType.Player)
        {

            switch (actionType)
            {
                case BattleActionType.Start:
                    player.GetComponent<Animator>().SetTrigger("battle");
                    break;
                case BattleActionType.Attack:
                    num = ReturnRandomFloat();
                    player.GetComponent<Animator>().SetTrigger("attack");
                    player.GetComponent<Animator>().SetFloat("attackAnim", num);
                    break;
                case BattleActionType.Damaged:
                    player.GetComponent<Animator>().SetTrigger("damaged");
                    StartCoroutine(ApplyDamageOrHeal(2, player, Color.red));
                    break;
                case BattleActionType.Healed:
                    StartCoroutine(ApplyDamageOrHeal(2, player, Color.green));
                    break;
                case BattleActionType.Gaurd:
                    player.GetComponent<Animator>().SetTrigger("gaurd");
                    break;
                case BattleActionType.StopGaurding:
                    player.GetComponent<Animator>().SetTrigger("stopGaurding");
                    break;
                case BattleActionType.Arcane:
                    player.GetComponent<Animator>().SetTrigger("arcane");
                    break;
                case BattleActionType.Run:
                    player.GetComponent<Animator>().SetTrigger("run");
                    break;
                case BattleActionType.Die:
                    player.GetComponent<Animator>().SetTrigger("die");
                    break;
                case BattleActionType.Won:
                    player.GetComponent<Animator>().SetTrigger("battleEnd");
                    break;
                default:
                    break;
            }
        }
        else
            return;
    }


    private float ReturnRandomFloat()
    {
        return Random.Range(0.0f, 1.0f);
    }

    /// <summary>
    /// Displays the damage/heal amount and turns it off after X seconds
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator ApplyDamageOrHeal(float waitTime, Unit player, Color textColor)
    {
        dmgText.color = textColor;
        player.damageNumbers.SetActive(true);
        dmgText.text = battleSystem?.LastDamage.ToString();
        dmgAnimator.SetTrigger("damaged");
        yield return new WaitForSeconds(waitTime);
        dmgText.color = Color.red;
        player.damageNumbers.SetActive(false);
    }
}
