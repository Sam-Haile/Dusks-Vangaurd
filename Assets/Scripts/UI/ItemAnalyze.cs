using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnalyze : MonoBehaviour
{
    public Transform itemLocation;
    private void Start()
    {
        this.transform.position = itemLocation.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(itemLocation.position, new Vector3(750f, 750f, 1f));
    }
}
