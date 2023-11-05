using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static string enemyTag;
    public Vector3 beforeBattlePos;

    private bool isBattleInitiated = false;

    public Animator battleTransitionAnimator;

    /// <summary>
    /// Save the players position in the main world
    /// Destroy the enemy and its spawner
    /// Begin loading the battle scene
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isBattleInitiated && other.gameObject.GetComponent<EnemyAI>())
        {
            beforeBattlePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            isBattleInitiated = true;
            enemyTag = other.gameObject.tag;

            EnemyAI enemyai = other.GetComponent<EnemyAI>();

            enemyai.refToSpawner.OnEnemyDestroyed(enemyai.refToSpawner.gameObject.name);

            StartCoroutine(LoadScene(other));
        }
    }

    IEnumerator LoadScene(Collider enemyToDestroy)
    {
        battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(2);
        isBattleInitiated = false;
    }


    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            battleTransitionAnimator.SetTrigger("Start");
            this.transform.position = beforeBattlePos;
        }
    }

}
