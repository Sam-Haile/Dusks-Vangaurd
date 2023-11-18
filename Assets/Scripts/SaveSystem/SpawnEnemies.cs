using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SpawnEnemies : MonoBehaviour
{
    public List<GameObject> enemies; // List to hold enemy GameObjects
    public List<float> preBattleSpeeds;
    public List<float> preBattleRotation;

    private void Awake()
    {
        PopulateEnemyList();
    }

    private void Start()
    {
        // Set active state of enemies based on their defeat status
        AddEnemiesToList();
    }

    private void Update()
    {
        UpdateEnemyActiveStates();
    }

    // This method populates the enemy list with all EnemyAI objects in the scene
    private void PopulateEnemyList()
    {
        EnemyAI[] enemyObjects = FindObjectsOfType<EnemyAI>();
        foreach (var enemyObject in enemyObjects)
        {
            enemies.Add(enemyObject.gameObject);
            preBattleSpeeds.Add(enemyObject.speed);
            preBattleRotation.Add(enemyObject.rotationSpeed);
        }
    }

    public void AddEnemiesToList()
    {
        foreach (GameObject enemy in enemies)
        {
            GameData.enemies.Add(enemy);
        }
    }

    // This method updates the active state of each enemy in the list
    public void UpdateEnemyActiveStates()
    {
        foreach (GameObject enemy in GameData.enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            
            if (enemyAI != null && enemyAI.isDefeated)
            {
                enemyAI.GetComponent<Collider>().enabled = false;
                enemy.SetActive(false);
            }
            else
            {
                enemyAI.GetComponent<Collider>().enabled = true;
                enemy.SetActive(true);
            }
        }
    }

    private void StopMoving()
    {
        foreach (var enemy in enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            Rigidbody rb = enemy.GetComponent<Rigidbody>(); 

            rb.useGravity = false;
            enemyAI.speed = 0;
            enemyAI.rotationSpeed = 0;
        }
    }

    private void ResetMovement()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyAI enemyAI = enemies[i].GetComponent<EnemyAI>();
            enemyAI.speed = preBattleSpeeds[i];
            enemyAI.rotationSpeed = preBattleRotation[i];

            Rigidbody rb = enemies[i].GetComponent<Rigidbody>();
            rb.useGravity = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // or check scene.name
        {
            ResetMovement();
        }
        else if (scene.buildIndex == 2)
        {
            StopMoving();
            UpdateEnemyActiveStates();
        }
    }

    //private void HideDefeatedEnemies()
    //{
    //    // Find all EnemyAI components in the scene
    //    EnemyAI[] enemyObjects = FindObjectsOfType<EnemyAI>();

    //    foreach (EnemyAI enemyAI in enemyObjects)
    //    {
    //        if (enemyAI.isDefeated)
    //        {
    //            enemyAI.gameObject.SetActive(false);
    //        }
    //    }

    //}

    // Call this method to reset the enemies after a battle or when restarting a level
    public void ResetEnemies()
    {
        foreach (var enemy in enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.isDefeated = false;
                enemy.SetActive(true);
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
