using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static string enemyTag;
    public Vector3 beforeBattlePos;

    // When player collides with an enemy
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.GetComponent<Unit>())
        {
            beforeBattlePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            enemyTag = collision.gameObject.tag;
            SceneManager.LoadScene(2);
        }
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            this.transform.position = beforeBattlePos;
        }
    }

}
