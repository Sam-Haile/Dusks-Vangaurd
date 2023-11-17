using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static string enemyTag;
    public Vector3 beforeBattlePos;

    private bool isBattleInitiated = false;

    public Animator battleTransitionAnimator;

    public bool canBattle = true;

    public MenuManager menuManager;

    /// <summary>
    /// Save the players position in the main world
    /// Destroy the enemy and its spawner
    /// Begin loading the battle scene
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {

        EnemyAI enemyReference = other.GetComponent<EnemyAI>();

        if (!isBattleInitiated && enemyReference && canBattle)
        {
            this.GetComponent<PlayerMovement>().enabled = false;
            beforeBattlePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            isBattleInitiated = true;
            enemyTag = other.gameObject.tag;
            canBattle = false;

            enemyReference.isDefeated = true;
            enemyReference.collider.enabled = !enemyReference.isDefeated;
            StartCoroutine(LoadScene(other.gameObject));

        }
    }

    IEnumerator LoadScene(GameObject enemyToDestroy)
    {
        battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(2);
    }


    public void OnLevelWasLoaded(int level)
    {
        if (level == 1 && isBattleInitiated)
        {
            //menuManager.LoadGameData()
            battleTransitionAnimator.SetTrigger("Start");
            StartCoroutine(BattleCooldown());
            this.transform.position = beforeBattlePos;
            isBattleInitiated = false; // Reset the flag after handling the scene load
        }
    }

    IEnumerator BattleCooldown()
    {
        yield return new WaitForSeconds(3f);
        canBattle = true;
    }

}
