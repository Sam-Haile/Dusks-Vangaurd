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
        FirstRoundSpawns();
    }

    private void FirstRoundSpawns()
    {
        string enemyId = gameObject.name; // Use the spawner's name as the enemy's identifier

        // If the enemy does not exist in the dictionary or is not destroyed, spawn it.
        Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, .25f, randomPosition.y);

        GameObject enemy = Instantiate(listOfEnemyPrefabs[Random.Range(enemyPrefabMin, enemyPrefabMax)], spawnPosition, Quaternion.identity);
        spawnedEnemy = enemy.GetComponent<EnemyAI>();
        spawnedEnemy.refToSpawner = this;
        spawnedEnemy.isSpawned = true;
        enemy.name = enemyId; // Set the enemy's name to the consistent identifier


        // Add the enemy to the GameData dictionary if it's not already there.
        if (!GameData.enemies.ContainsKey(enemyId))
        {
            Debug.Log("First sessdcsdcsion of spawning");
            GameData.enemies[enemyId] = false;
        }
    }

    public void SpawnEnemy()
    {
        if (canSpawn)
        {

            string enemyId = gameObject.name; // Use the spawner's name as the enemy's identifier

            // If the enemy does not exist in the dictionary or is not destroyed, spawn it.
            Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, .25f, randomPosition.y);

            if (spawnedEnemy == null)
            {
                GameObject enemy = Instantiate(listOfEnemyPrefabs[Random.Range(enemyPrefabMin, enemyPrefabMax)], spawnPosition, Quaternion.identity);
                spawnedEnemy = enemy.GetComponent<EnemyAI>();
                spawnedEnemy.refToSpawner = this;
                enemy.name = enemyId; // Set the enemy's name to the consistent identifier
            }

            // Add the enemy to the GameData dictionary if it's not already there.
            if (!GameData.enemies.ContainsKey(enemyId))
            {
                Debug.Log("First session of spawning");
                GameData.enemies[enemyId] = false;
            }
            // if it exists in the dictionary slot
            // respawn it, 
            else if (GameObject.Find(enemyId) != null)
            {
                Debug.Log("Second session of spawning");
                return; // Enemy already exists in the scene, don't spawn another.
            }
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
