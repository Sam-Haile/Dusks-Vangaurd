using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using TMPro;

public static class SaveSystem {


    public static void SavePlayer(int saveSlotID, PlayableCharacter player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string fileNamePrefix = "/saveSlot_" + saveSlotID + "_" + player.name + ".sve";
        string path = Application.persistentDataPath + fileNamePrefix;
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }


    public static PlayerData LoadPlayer(int saveSlotID, string playerName)
    {
        string fileNamePrefix = "/saveSlot_" + saveSlotID + "_" + playerName + ".sve";

        string path = Application.persistentDataPath + fileNamePrefix;
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

    public static void SaveGameData(int saveSlotID, string enemyId, bool isDefeated)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string fileNamePrefix = "/saveSlot_" + saveSlotID + "_enemies.sve";
        string path = Application.persistentDataPath + fileNamePrefix;
        FileStream stream = new FileStream(path, FileMode.Create);

        EnemyData data = new EnemyData();

        foreach (var enemy in GameData.enemies)
        {
            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {

                Vector3 pos = enemy.transform.position;
                Quaternion rot = enemy.transform.rotation;
                float[] posArray = new float[] { pos.x, pos.y, pos.z };
                float[] rotArray = new float[] { rot.x, rot.y, rot.z, rot.w };

                data.AddEnemyState(enemyAI.enemyID, isDefeated, posArray, rotArray);
            }
        }

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static EnemyData LoadGameData(int saveSlotID)
    {
        string fileNamePrefix = "/saveSlot_" + saveSlotID + "_enemies.sve";
        string path = Application.persistentDataPath + fileNamePrefix;
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
