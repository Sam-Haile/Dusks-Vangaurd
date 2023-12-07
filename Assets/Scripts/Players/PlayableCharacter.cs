using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayableCharacter : Unit
{

    public int expToNextLevel = 100;

    public Button weaponSlot;
    public Button armorSlot;

    public Spellbook spellbook;
    public List<ParticleSystem> spellParticles;

    public bool isActive;

    public GameObject uiHUD;
    public bool isDead = false;
    
    public void AddExperience(int exp)
    {
        experience += exp;

        // Check for level up
        if (experience >= expToNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        unitLevel++;

        int overflowExp = experience - expToNextLevel;

        experience = overflowExp;
        //experience -= expToNextLevel;

        // Increase expToNextLevel by some amount, could be a fixed value or a function of the current level
        expToNextLevel += 100;

        // Update player stats as needed...

        // Show the level up screen
        // This would need to communicate with a UI manager script to display the level up screen
    }

}
