using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public List<GameObject> enemies; // List to hold enemy GameObjects

    private void Awake()
    {
        // Optionally, populate the list if not done through the Unity Inspector
        // This can be done by finding all objects of type EnemyAI, for example
        //PopulateEnemyList();
    }

    private void Start()
    {
        // Set active state of enemies based on their defeat status
        AddEnemiesToList();
    }

    private void Update()
    {
        Debug.Log(GameData.enemies.Count);
        UpdateEnemyActiveStates();
    }

    // This method populates the enemy list with all EnemyAI objects in the scene
    //private void PopulateEnemyList()
    //{
    //    EnemyAI[] enemyObjects = FindObjectsOfType<EnemyAI>();
    //    foreach (var enemyObject in enemyObjects)
    //    {
    //        enemies.Add(enemyObject.gameObject);
    //    }
    //}

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
                enemyAI.collider.enabled = false;
                enemy.SetActive(false);
            }
            else
            {
                enemyAI.collider.enabled = true;
                enemy.SetActive(true);
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            //HideDefeatedEnemies();
        }
        if(level == 2)
        {
            UpdateEnemyActiveStates();
        }

    }

    private void HideDefeatedEnemies()
    {
        // Find all EnemyAI components in the scene
        EnemyAI[] enemyObjects = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemyAI in enemyObjects)
        {


            if (enemyAI.isDefeated)
            {
                enemyAI.gameObject.SetActive(false);
            }
        }

    }

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
}
