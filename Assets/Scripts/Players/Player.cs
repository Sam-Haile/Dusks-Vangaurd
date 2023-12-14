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
        ParticleSystem smite = spellParticles.Find(p => p.name == "Smite");
        spellbook.LearnSpell(new Spell("Smite", 5, 15, 20, false, smite));

    }

}
