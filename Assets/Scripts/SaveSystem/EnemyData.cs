using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Dictionary<string, bool> enemies = new Dictionary<string, bool>();
    public Dictionary<string, float[]> enemyPositions = new Dictionary<string, float[]>();
    public Dictionary<string, float[] > enemyRotations = new Dictionary<string, float[]>();

    public void AddEnemyState(string enemyId, bool defeated, float[] position, float[] rotation)
    {
        enemies[enemyId] = defeated;

        enemyPositions[enemyId] = position;
        enemyRotations[enemyId] = rotation;
    }
}
