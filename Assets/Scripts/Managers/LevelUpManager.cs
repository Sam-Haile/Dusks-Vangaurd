using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    BattleSystem battleSystem;

    private void Awake()
    {
        battleSystem = GetComponent<BattleSystem>();
        battleSystem.levelUpEvent.AddListener(LevelUp);
    }

    public void LevelUp()
    {
        //Do level up stuff here
        Debug.Log("SDCSC");
    }

    private void OnDisable()
    {
        
    }
}
