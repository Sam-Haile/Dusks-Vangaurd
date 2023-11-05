using System.Collections.Generic;
using UnityEngine;

/*Used for spawning enemies in the main world*/
public class Spawner : MonoBehaviour
{
    public int enemyPrefabMin;
    public int enemyPrefabMax;

    public List<GameObject> listOfEnemyPrefabs;

    public bool canSpawn;

    public float spawnRadius = 10f;

    public PlayerCollision playerCollision;

    private EnemyAI spawnedEnemy;

    private void Start()
    {
        if (canSpawn)
        {
            SpawnEnemy();
        }
    }


    private void SpawnEnemy()
    {
        string enemyId = gameObject.name; // Use the spawner's name as the enemy's identifier

        // Check if the enemy has already been destroyed.
        // If it has, do not spawn it again.
        if (GameData.enemies.TryGetValue(enemyId, out bool isDestroyed) && isDestroyed)
        {
            return; // Enemy was already destroyed, don't spawn it.
        }

        // Check if the enemy has been destroyed already
        Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, 1f, randomPosition.y);

        GameObject enemy = Instantiate(listOfEnemyPrefabs[Random.Range(enemyPrefabMin, enemyPrefabMax)], spawnPosition, Quaternion.identity);
        spawnedEnemy = enemy.GetComponent<EnemyAI>();
        spawnedEnemy.refToSpawner = this;
        enemy.name = enemyId; // Set the enemy's name to the consistent identifier

        // Add the enemy to the GameData dictionary if it's not already there.
        if (!GameData.enemies.ContainsKey(enemyId))
        {
            GameData.enemies[enemyId] = false;
        }
    }

    // Call this method when the enemy is destroyed
    public void OnEnemyDestroyed(string enemyId)
    {
        GameData.enemies[enemyId] = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }


}
