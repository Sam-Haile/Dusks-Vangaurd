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

    // When player collides with an enemy

    private void OnTriggerEnter(Collider other)
    {
        if (!isBattleInitiated && other.gameObject.GetComponent<Unit>())
        {
            isBattleInitiated = true;
            beforeBattlePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            enemyTag = other.gameObject.tag;
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(.75f);
        SceneManager.LoadSceneAsync(2);
        isBattleInitiated = false;
    }


    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            this.transform.position = beforeBattlePos;
        }
    }

}
