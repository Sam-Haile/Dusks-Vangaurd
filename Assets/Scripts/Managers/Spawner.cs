using System.Collections.Generic;
using UnityEngine;

/*Used for spawning enemies in the main world*/
public class Spawner : MonoBehaviour
{
    public int enemyPrefabMin;
    public int enemyPrefabMax;

    public List<GameObject> listOfEnemyPrefabs;

    public bool canSpawn = true;

    public float spawnRadius = 10f;

    // Dictionary to keep track of destroyed enemies
    private Dictionary<string, bool> enemies = new Dictionary<string, bool>();

    private void Start()
    {
        if (canSpawn)
        {

            Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, 1f, randomPosition.y);

            string enemyId = System.Guid.NewGuid().ToString();

            // Check if the enemy has been destroyed already
            if (!enemies.ContainsKey(enemyId) || !enemies[enemyId])
            {
                GameObject enemy = Instantiate(listOfEnemyPrefabs[Random.Range(enemyPrefabMin, enemyPrefabMax)], spawnPosition, Quaternion.identity);
                enemy.GetComponent<EnemyAI>().refToSpawner = this;
                enemy.name = enemyId;  // Assign the unique ID to the enemy
                enemies.Add(enemyId, false);  // Add the enemy to the list
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

}
