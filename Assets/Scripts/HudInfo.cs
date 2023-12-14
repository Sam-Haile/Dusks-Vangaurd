using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudInfo : MonoBehaviour
{
    //Fields for a HUD in the battle scene

    // used for both
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI maxHP;
    public TextMeshProUGUI currentHP;
    public TextMeshProUGUI maxMP;
    public TextMeshProUGUI currentMP;
    
    // used for inventory
    public TextMeshProUGUI level;
    public TextMeshProUGUI currentEXP;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI arcane;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI spcDefense;
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI armor;
    public TextMeshProUGUI xpNeeded;
    // used for battle
    public Slider hpSlider;
    public Slider mpSlider;
    public Image selectedUnit;

    public Image playerSpriteHead;
    public Image playerSpriteFull;

    public void UpdateBattleStats(Unit unit)
    {
        characterName.text = unit.unitName;

        maxHP.text = "/" + unit.maxHP.ToString();
        currentHP.text = unit.currentHP.ToString();

        maxMP.text = "/" + unit.maxMP.ToString();
        currentMP.text = unit.currentMP.ToString();

        hpSlider.minValue = 0;
        hpSlider.maxValue = 1;
        hpSlider.value = (float)unit.currentHP / unit.maxHP;

        mpSlider.minValue = 0;
        mpSlider.maxValue = 1;
        mpSlider.value = (float)unit.currentMP / unit.maxMP;
    }

    public void UpdateInventory(PlayableCharacter unit)
    {
        characterName.text = unit.unitName;
        level.text = unit.unitLevel.ToString();
        currentEXP.text = unit.experience.ToString();
        maxHP.text = unit.maxHP.ToString();
        currentHP.text = unit.currentHP.ToString();
        attack.text = unit.baseAttack.ToString();
        arcane.text = unit.baseArcane.ToString();
        defense.text = unit.baseDefense.ToString();
        xpNeeded.text = unit.expToNextLevel.ToString();
        spcDefense.text = unit.specialDefense.ToString();
        playerSpriteFull.sprite = unit.playerSpriteFull;
        level.text = unit.unitLevel.ToString();
    }

    public void UpdatePartyMenu(PlayableCharacter player)
    {
        characterName.text = player.unitName;
        level.text = player.unitLevel.ToString();
        currentHP.text = player.currentHP.ToString();
        maxHP.text = "/" + player.maxHP.ToString();
        hpSlider.value = player.currentHP / player.maxHP;
        mpSlider.value = player.currentMP / player.maxMP;
        playerSpriteFull.sprite = player.playerSpriteFull;
    }
}
