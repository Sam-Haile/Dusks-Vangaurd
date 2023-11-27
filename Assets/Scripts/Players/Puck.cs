using UnityEngine;

public class Puck : PlayableCharacter
{
    public static Puck instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        //ParticleSystem healingDust = spellParticles.Find(p => p.name == "Healing Dust");
        //spellbook.LearnSpell(new Spell("Heal", 3, 5, 10, true, healingDust));

        //ParticleSystem fireclaws = spellParticles.Find(p => p.name == "Fireclaws");
        //spellbook.LearnSpell(new Spell("Fireball", 10, 20, 30, false, fireclaws));
    }

}
