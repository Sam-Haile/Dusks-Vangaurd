using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpStats : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI[] previousStats;
    public TextMeshProUGUI[] newStats;
    public Animator animator;
    public LevelUpManager lvlUpManager;

    public void UpdateStats(PlayableCharacter player)
    {
        playerName.text = player.unitName;
        player.unitLevel++;

        previousStats[0].text = player.maxHP.ToString(); // hp
        previousStats[1].text = player.maxMP.ToString(); // mp
        previousStats[2].text = player.baseAttack.ToString(); // attack
        previousStats[3].text = player.baseArcane.ToString(); // arcane
        previousStats[4].text = player.baseDefense.ToString(); // defense
        previousStats[5].text = player.specialDefense.ToString(); // specialDefense


        // Update each stat by 1 or 2 for now
        for (int i = 0; i < previousStats.Length; i++)
        {
            int stat = 0;
            Int32.TryParse(previousStats[i].text, out stat);
            stat += UnityEngine.Random.Range(1, 3);
            newStats[i].text = stat.ToString();
        }
        
        Int32.TryParse(newStats[0].text, out player.maxHP);
        Int32.TryParse(newStats[1].text, out player.maxMP);
        Int32.TryParse(newStats[2].text, out player.baseAttack);
        Int32.TryParse(newStats[3].text, out player.baseArcane);
        Int32.TryParse(newStats[4].text, out player.baseDefense);
        Int32.TryParse(newStats[5].text, out player.specialDefense);

        // Restore players health and magic when leveling up
        player.currentHP = player.maxHP;
        player.currentMP = player.maxMP;
    }

    public void TriggerStatUpdate()
    {
        UpdateStats(lvlUpManager.currentPlayer);
    }
}

