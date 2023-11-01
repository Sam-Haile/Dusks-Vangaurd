using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Slider hpSlider;
	public Slider hpSlider2;
	public TextMeshProUGUI money;

	public void SetHUD(Unit unit1, Unit unit2)
	{
        string amount = unit1.money.ToString();
        money.text = amount.ToString();
    }

	public void SetHP(Unit unit, int player)
	{
		if(player == 1) 
		{
			hpSlider.minValue= 0;
			hpSlider.maxValue = 1;
			hpSlider.value = (float)unit.currentHP / unit.maxHP;
            Debug.Log(hpSlider.value);
        }

        else if (player == 2)
        {
            hpSlider2.minValue = 0;
            hpSlider2.maxValue = 1;
            hpSlider2.value = (float)unit.currentHP / unit.maxHP;
			Debug.Log(hpSlider2.value);
        }
    }

}
