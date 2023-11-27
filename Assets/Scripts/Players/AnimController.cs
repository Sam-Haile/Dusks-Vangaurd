using System.Collections;
using TMPro;
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
        if (level == 2)
        {
            battleSystem = FindAnyObjectByType<BattleSystem>();
            hasBattled = true;
            playerAnimator.SetTrigger("battle");
            player2Animator.SetTrigger("battle");
            swordBack.SetActive(false);
            swordHand.SetActive(true);
        }
        else if(level == 1)
        {
            swordBack.SetActive(true);
            swordHand.SetActive(false);

            if (hasBattled)
            {
                playerAnimator.SetTrigger("battleEnd");
                player2Animator.SetTrigger("battleEnd");
            }

            hasBattled = false;
        }
    }

    private void HandlePlayerAction(BattleActionType actionType, Unit player)
    {
        dmgAnimator = player.damageNumbers.GetComponent<Animator>();
        dmgText = player.damageNumbers.GetComponent<TextMeshPro>();

        switch (actionType)
        {
            case BattleActionType.Start:
                
                break;
            case BattleActionType.Attack:
                if(player.tag == "Player")
                {
                    num = ReturnRandomFloat();
                    playerAnimator.SetTrigger("attack");
                    playerAnimator.SetFloat("attackAnim", num);
                }
                else if(player.tag == "Puck")
                    player2Animator.SetTrigger("attack");
                break;
            case BattleActionType.Damaged:
                if (player.tag == "Player"){
                    playerAnimator.SetTrigger("damaged");
                }
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("damaged");
                StartCoroutine(ApplyDamageOrHeal(2, player, Color.red));
                break;
            case BattleActionType.Healed:
                StartCoroutine(ApplyDamageOrHeal(2, player, Color.green));
                break;
            case BattleActionType.Gaurd:
                if (player.tag == "Player")
                    playerAnimator.SetTrigger("gaurd");
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("gaurd");
                break;
            case BattleActionType.StopGaurding:
                if (player.tag == "Player")
                    playerAnimator.SetTrigger("stopGaurding");
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("stopGaurding");
                break;
            case BattleActionType.Arcane:
                if (player.tag == "Player")
                    playerAnimator.SetTrigger("arcane");
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("arcane");
                break;
            case BattleActionType.Run:
                if (player.tag == "Player")
                    playerAnimator.SetTrigger("run");
                else if (player.tag == "Player")
                    player2Animator.SetTrigger("run");
                break;
            case BattleActionType.Die:
                if (player.tag == "Player")
                    playerAnimator.SetTrigger("die");
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("die");
                break;
            case BattleActionType.Won:
                if(player.tag == "Player")
                    playerAnimator.SetTrigger("battleEnd");
                else if (player.tag == "Puck")
                    player2Animator.SetTrigger("battleEnd");
                break;
            default:
                break;
        }
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
