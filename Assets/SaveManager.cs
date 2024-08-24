using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string saveFilePath => Application.persistentDataPath + "/playerData.json";

    // Save player data to a JSON file
    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    // Load player data from the JSON file
    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return new PlayerData();  // Return default data if no save file exists
    }
}
