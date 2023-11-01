[System.Serializable]
public class PlayerData
{

    //Player fields
    public string unitName;
    public int unitLevel;
    public int maxHP;
    public int currentHP;
    public int baseAttack;
    public int baseArcane;
    public int baseDefense; 
    public int specialDefense;
    public int experience;
    public int money;
    
    public int expToNextLevel;
    public float[] position;
    public float[] rotation;

    public PlayerData(PlayableCharacter player) 
    {
        unitName = player.unitName;
        unitLevel = player.unitLevel;
        maxHP = player.maxHP;
        currentHP = player.currentHP;
        baseAttack = player.baseAttack;
        baseArcane = player.baseArcane;
        baseDefense = player.baseDefense;
        specialDefense = player.specialDefense;
        experience = player.experience;
        money = player.money;
        expToNextLevel = player.expToNextLevel;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;


        rotation = new float[3];
        rotation[0] = player.transform.rotation.eulerAngles.x;
        rotation[1] = player.transform.rotation.eulerAngles.y;
        rotation[2] = player.transform.rotation.eulerAngles.z;
    }



}
