using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
