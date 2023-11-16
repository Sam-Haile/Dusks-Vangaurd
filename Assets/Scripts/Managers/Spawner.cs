//using System.Collections.Generic;
//using UnityEngine;

///* Used for spawning enemies in the main world */
//public class Spawner : MonoBehaviour
//{
//    public int enemyPrefabMin;
//    public int enemyPrefabMax;

//    public List<GameObject> listOfEnemyPrefabs;

//    public float spawnRadius = 10f;

//    public PlayerCollision playerCollision;

//    public EnemyAI spawnedEnemy;

//    public bool canSpawn;


//    private void Start()
//    {
//        SpawnEnemyStart();
//    }

//    private void SpawnEnemyStart()
//    {
//        if (canSpawn)
//        {
//                // Randomly pick an enemy prefab
//                int index = Random.Range(enemyPrefabMin, Mathf.Min(enemyPrefabMax, listOfEnemyPrefabs.Count));
//                GameObject enemyPrefab = listOfEnemyPrefabs[index];

//                // Generate a random position within the spawn radius
//                Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
//                spawnPosition += transform.position;
//                spawnPosition.y = 0; // Assuming you're spawning on a flat surface

//                // Instantiate the enemy
//                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

//                // Set up references to each other
//                spawnedEnemy = enemy.GetComponent<EnemyAI>();
//                spawnedEnemy.name = this.name;

//                // add enemy and set as true for alive
//                GameData.enemies.Add(spawnedEnemy, true);
//        }

//    }


//    public void RespawnEnemies()
//    {
//        Debug.Log("Destroying enemies");
//        foreach(EnemyAI enemy in GameData.enemies.Keys)
//        {
//            if (GameData.enemies[enemy] == false)
//            {
//                Destroy(enemy.gameObject);
//            }
//        }
//    }


//    private void OnLevelWasLoaded(int level)
//    {
//        if (level == 1)
//        {
//            RespawnEnemies();
//        }
//    }


//    // Call this method when the enemy is destroyed
//    public void OnEnemyDefeated(EnemyAI enemyId)
//    {
//        GameData.enemies[enemyId] = false;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, spawnRadius);
//    }
//}
