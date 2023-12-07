using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UnitType {Player, Enemy }

public class Unit : MonoBehaviour
{
    public UnitType Type { get; set; }

    public event Action OnStatsChanged;

    public string unitName;
	public int unitLevel;

	public int maxHP;
	public int currentHP;
	public int maxMP;
	public int currentMP;
	public int baseAttack;
	public int baseArcane;
	public int baseDefense; // For physical attacks
	public int specialDefense; // For arcane attacks
    public int experience;
	public int gold;

	public bool canUseMagic;
    public Weapon equippedWeapon;
    public Armor equippedArmor;

	public GameObject damageNumbers;

    public int CurrentHealth //Update the UI when a stat changes
    {
        get { return currentHP; }
        set
        {
            currentHP = value;
            OnStatsChanged?.Invoke();
        }
    }

    public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}
}
