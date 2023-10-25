using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPositions : MonoBehaviour
{
    public void LoadPosition(Vector3 goTo)
    {
        Debug.Log("Going from " + this.transform.position + " to " + goTo);
        this.transform.position = goTo;
    }
}
