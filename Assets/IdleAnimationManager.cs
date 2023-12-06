using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimationManager : MonoBehaviour
{
    public Animation anim;

    private void Start()
    {
    }

    public void PlayIdle()
    {
        Debug.Log("Trying to play anim");
        anim.Play("LeftTurn");
    }


}
