using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Slider hpSlider;
	public Slider hpSlider2;
	public Slider mpSlider;
	public Slider mpSlider2;
	public TextMeshProUGUI money;

	public void SetHUD(Unit unit1, Unit unit2)
	{
        string amount = unit1.money.ToString();
        money.text = amount.ToString();
    }

	public void SetHP(Unit unit)
	{
		if(unit.tag == "Player") 
		{
			hpSlider.minValue= 0;
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

}
