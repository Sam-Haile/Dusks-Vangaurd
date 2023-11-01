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
		hpSlider.maxValue = unit1.maxHP;
		hpSlider.value = unit1.currentHP;

        hpSlider2.maxValue = unit2.maxHP;
        hpSlider2.value = unit2.currentHP;

        string amount = unit1.money.ToString();
        money.text = amount.ToString();
    }

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
	}

}
