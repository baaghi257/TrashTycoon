using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class GameSaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/gamedata";
    }

    public void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(saveFilePath);

        formatter.Serialize(file, data);
        file.Close();
    }

    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);

            GameData data = (GameData)formatter.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            return new GameData();
        }
    }
}
