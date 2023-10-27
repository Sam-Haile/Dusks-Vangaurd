using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Slider hpSlider;

	public void SetHUD(Unit unit)
	{
		hpSlider.maxValue = unit.maxHP;
		hpSlider.value = unit.currentHP;
	}

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
	}

}
