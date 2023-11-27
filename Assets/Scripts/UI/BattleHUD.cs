using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

    public Slider hpSlider;
    public Slider hpSlider2;
    public Slider mpSlider;
    public Slider mpSlider2;
    public TextMeshProUGUI gold;

    public Unit guts;
    public Unit puck;
    private void Awake()
    {
        guts = FindObjectOfType<Player>();
        puck = FindObjectOfType<Puck>();
    }

    public void SetHUD()
    {
        string amount = guts.gold.ToString();
        gold.text = amount.ToString();
    }

    public void SetHP(Unit unit)
    {
        if (unit.tag == "Player")
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = 1;
            hpSlider.value = (float)unit.currentHP / unit.maxHP;
        }

        else if (unit.tag == "Puck")
        {
            hpSlider2.minValue = 0;
            hpSlider2.maxValue = 1;
            hpSlider2.value = (float)unit.currentHP / unit.maxHP;
        }
    }

    public void SetMP(Unit unit)
    {
        if (unit.tag == "Player")
        {
            mpSlider.minValue = 0;
            mpSlider.maxValue = 1;
            mpSlider.value = (float)unit.currentMP / unit.maxMP;
        }

        else if (unit.tag == "Puck")
        {
            mpSlider2.minValue = 0;
            mpSlider2.maxValue = 1;
            mpSlider2.value = (float)unit.currentMP / unit.maxMP;
        }
    }

    public void UpdateAllStats(Unit unit)
    {
        switch (unit.tag)
        {
            case "Player":
                string amount = unit.gold.ToString();
                gold.text = amount.ToString();
                hpSlider.minValue = 0;
                hpSlider.maxValue = 1;
                hpSlider.value = (float)unit.currentHP / unit.maxHP;

                mpSlider.minValue = 0;
                mpSlider.maxValue = 1;
                mpSlider.value = (float)unit.currentMP / unit.maxMP;

                //Debug.Log("Setting " + mpSlider.name + " from " + mpSlider.value + " to " + (float)unit.currentMP / unit.maxMP);
                break;
            case "Puck":
                hpSlider2.minValue = 0;
                hpSlider2.maxValue = 1;
                hpSlider2.value = (float)unit.currentHP / unit.maxHP;

                mpSlider2.minValue = 0;
                mpSlider2.maxValue = 1;
                mpSlider2.value = (float)unit.currentMP / unit.maxMP;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        //Debug.Log("Current MP" + guts.currentMP);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            UpdateAllStats(guts);
            UpdateAllStats(puck);
        }
    }

    IEnumerator UpdateStats()
    {
        yield return new WaitForSeconds(3f);
        UpdateAllStats(guts);
    }

}
