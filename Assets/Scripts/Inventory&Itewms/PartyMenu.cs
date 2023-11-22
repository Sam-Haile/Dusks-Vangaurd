using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenu : MonoBehaviour
{

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI health;
    public Slider hpSlider;
    public Slider mpSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStats(PlayableCharacter player)
    {
        playerName.text = player.unitName;
        level.text = player.unitLevel.ToString();
        health.text = player.currentHP.ToString() + "/ " + player.maxHP;

        hpSlider.value = player.currentHP / player.maxHP;
        mpSlider.value = player.currentMP / player.maxMP;
    }
}
