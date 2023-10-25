using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            SceneManager.LoadScene(1);
        }
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            this.transform.position = beforeBattlePos;
        }
    }

}
