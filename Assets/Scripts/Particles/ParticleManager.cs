using System.Collections;
using UnityEngine;
using static BattleSystem;

public class ParticleManager : MonoBehaviour
{
    public GameObject attack1Particles;
    public GameObject attack2Particles;
    public GameObject attack3Particles;

    public GameObject battleStart;

    private AnimController animController;
    private float animControllerNum;

    private void Start()
    {
        animController = FindObjectOfType<AnimController>();
    }

    private void OnEnable()
    {
        BattleSystem.OnBattleAction += HandleParticles;
    }

    private void OnDisable()
    {
        BattleSystem.OnBattleAction -= HandleParticles;
    }

    private void HandleParticles(BattleActionType actionType, Unit player, UnitType u)
    {
        if (u == UnitType.Player)
        {

            switch (actionType)
            {
                case BattleActionType.Start:
                    if (player.tag == "Player") { }
                    StartCoroutine(PlayParticles(battleStart, 1.25f, 3f));
                    break;
                case BattleActionType.Attack:
                    if (player.tag == "Player")
                    {
                        //animControllerNum = animController.GetNumValue();
                        //if (animControllerNum < .33)
                        //else if (animControllerNum > .33 && animControllerNum < .66)
                        //{
                        //}
                        //else
                        //{
                        //}
                    }
                    break;
                case BattleActionType.Gaurd:
                    break;
                case BattleActionType.Arcane:
                    break;
                case BattleActionType.Run:
                    break;
                case BattleActionType.Die:
                    break;
                default:
                    break;
            }
        }
        else
            return;
    }

    IEnumerator PlayParticles(GameObject particles, float initialWait, float finalWaitTime)
    {
        yield return new WaitForSeconds(initialWait);
        particles.SetActive(true);
        yield return new WaitForSeconds(finalWaitTime);
        particles.SetActive(false);
    }

}
