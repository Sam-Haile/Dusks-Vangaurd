using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static string enemyTag;
    public Vector3 beforeBattlePos;


    // When player collides with an enemy
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.GetComponent<Unit>() != null && collision.gameObject.tag != "Puck")
        {
            beforeBattlePos = this.transform.position;
            enemyTag = collision.gameObject.tag;
            SceneManager.LoadScene(1);
        }
    }


}
