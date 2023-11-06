using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{

    public List<Spell> spells;

    // Method to add a spell to the spellbook
    public void LearnSpell(Spell newSpell)
    {
        spells.Add(newSpell);
    }

    // Method to cast a spell by name
    public int CastSpell(string spellName, Unit player)
    {
        Spell spellToCast = spells.Find(spell => spell.spellName == spellName);
        if (spellToCast != null && player.currentMP >= spellToCast.mpCost)
        {
            // Subtract the MP cost
            player.currentMP -= spellToCast.mpCost;
            // Cast the spell and return its effect
            return spellToCast.Cast();
        }
        else
        {
            Debug.Log("Not enough MP or spell not found");
            return 0;
        }
    }
}
