using UnityEngine;

[System.Serializable]
public class Spell
{
    public string spellName;
    public int mpCost;
    public int minDamage;
    public int maxDamage;
    public bool isHealingSpell;

    public ParticleSystem spellVFX;

    // Constructor to set up a spell
    public Spell(string name, int cost, int minDmg, int maxDmg, bool healing, ParticleSystem particleSystem)
    {
        spellName = name;
        mpCost = cost;
        minDamage = minDmg;
        maxDamage = maxDmg;
        isHealingSpell = healing;
        spellVFX = particleSystem;
    }

    // Cast the spell and return the damage or healing amount
    public int Cast()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }
}
