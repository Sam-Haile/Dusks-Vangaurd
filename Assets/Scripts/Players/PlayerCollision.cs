using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static string enemyTag;
    public Vector3 beforeBattlePos;

    private bool isBattleInitiated = false;
    private GameObject enemy;

    private Spawner enemySpawner;

    public Animator battleTransitionAnimator;

    // When player collides with an enemy

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
            enemySpawner = other.GetComponent<EnemyAI>().refToSpawner;
            isBattleInitiated = true;
            enemyTag = other.gameObject.tag;
            StartCoroutine(LoadScene(other.gameObject));
        }
    }

    IEnumerator LoadScene(GameObject enemyToDestroy)
    {
        battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(3f);
        Destroy(enemyToDestroy);
        SceneManager.LoadSceneAsync(2);
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
