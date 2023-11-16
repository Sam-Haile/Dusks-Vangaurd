using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public bool[] activeEnemies;
    public Dictionary<string, bool> enemies = new Dictionary<string, bool>();

    public EnemyData(Spawner[] spawner, Dictionary<string, bool> currentEnemyStates)
    {
        activeEnemies = new bool[spawner.Length];

        for(int i = 0; i < spawner.Length; i++)
        {
            activeEnemies[i] = spawner[i].canSpawn;
        }

        // Create a new dictionary based on the provided currentEnemyStates
        enemies = new Dictionary<string, bool>(currentEnemyStates);
    }

}
