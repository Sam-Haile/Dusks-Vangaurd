[System.Serializable]
public class EnemyData
{
    public bool[] activeEnemies;

    public EnemyData(Spawner[] spawner)
    {
        activeEnemies = new bool[spawner.Length];

        for(int i = 0; i < spawner.Length; i++)
        {
            activeEnemies[i] = spawner[i].canSpawn;
        }
    }
}
