using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem fairyDust;

    private void Update()
    {
        if(rb.velocity.magnitude > 0)
        {
            fairyDust.Play();
        }
        else
        {
            fairyDust.Pause();
        }
    }
}
