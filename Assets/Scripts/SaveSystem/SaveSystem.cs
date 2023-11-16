using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using TMPro;

public static class SaveSystem {


    public static void SavePlayer(PlayableCharacter player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player_save_" + player.name + ".sve";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }


    public static PlayerData LoadPlayer(string playerName)
    {
        string path = Application.persistentDataPath + "/player_save_" + playerName + ".sve";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveEnemy(string enemyId, bool isDefeated)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/enemies.sve";
        FileStream stream = new FileStream(path, FileMode.Create);

        EnemyData data = new EnemyData();
        // Assuming you have a way to get all the enemies' states
        foreach (var enemy in GameData.enemies)
        {
            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                data.AddEnemyState(enemyId, isDefeated);
            }
        }

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static EnemyData LoadEnemies()
    {
        string path = Application.persistentDataPath + "/enemies.sve";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            EnemyData data = formatter.Deserialize(stream) as EnemyData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

}
