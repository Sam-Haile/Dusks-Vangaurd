using UnityEngine;

public class Player : PlayableCharacter
{

    public static Player instance;


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
    private void Start()
    {
        ParticleSystem fireclaws = spellParticles.Find(p => p.name == "Fireclaws");
        spellbook.LearnSpell(new Spell("Fireclaws", 10, 20, 30, false, fireclaws));

    }

}
