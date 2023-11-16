using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public Dictionary<string, bool> enemies = new Dictionary<string, bool>();

    public void AddEnemyState(string enemyId, bool defeated)
    {
        enemies[enemyId] = defeated;
    }
}
