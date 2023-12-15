using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTargetToGroup : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    void Awake()
    {
        targetGroup = GameObject.Find("TargetGroupPlayer").GetComponent<CinemachineTargetGroup>();

        //Add members to follow group
        if(Player.instance.transform!= null)
            targetGroup.AddMember(Player.instance.transform, 1f, 0f);
    }

}
