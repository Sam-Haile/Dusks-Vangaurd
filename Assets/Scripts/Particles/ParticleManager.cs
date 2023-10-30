using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleSystem;

public class ParticleManager : MonoBehaviour
{
    public GameObject attack1Particles;
    public GameObject attack2Particles;
    public GameObject attack3Particles;

    public GameObject battleStart;

    private AnimController animController;
    private float num;

    private void Start()
    {
        animController = FindObjectOfType<AnimController>();
    }

    private void OnEnable()
    {
        BattleSystem.OnPlayerAction += HandleParticles;
    }

    private void OnDisable()
    {
        BattleSystem.OnPlayerAction -= HandleParticles;
    }

    private void HandleParticles(BattleActionType actionType, int playerNumber)
    {
        switch (actionType)
        {
            case BattleActionType.Start:
                if(playerNumber == 1)
                    StartCoroutine(PlayParticles(battleStart, 1.25f, 3f));
                break;
            case BattleActionType.Attack:
                if (playerNumber == 1)
                {
                    num = .89f;
                    Debug.Log(num);
                    if (num < .33)
                        StartCoroutine(PlayParticles(attack1Particles, 1.15f, 1f));
                    else if (num > .33 && num < .66)
                    {
                        StartCoroutine(PlayParticles(attack1Particles, 1.15f, 1f));
                        StartCoroutine(PlayParticles(attack2Particles, 1.75f, 1f));
                    }
                    else
                    {
                        StartCoroutine(PlayParticles(attack1Particles, 1.15f, 1f));
                        StartCoroutine(PlayParticles(attack2Particles, 1.75f, 1f));
                        StartCoroutine(PlayParticles(attack3Particles, 2.5f, 3f));
                    }
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

    IEnumerator PlayParticles(GameObject particles, float initialWait, float finalWaitTime)
    {
        yield return new WaitForSeconds(initialWait);
        particles.SetActive(true);
        yield return new WaitForSeconds(finalWaitTime);
        particles.SetActive(false);
    }

}