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

    public Sprite playerSpriteHead;
    public Sprite playerSpriteFull;


    public void AddExperience(int exp)
    {
        experience += exp;
    }


}
