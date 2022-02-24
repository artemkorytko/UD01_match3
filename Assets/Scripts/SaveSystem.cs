using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private const string _dataKey = "GAME_DATA";

    public GameData LoadData()
    {
        if (PlayerPrefs.HasKey(_dataKey))
        {
            return JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(_dataKey));
        }
        else
        {
            return new GameData();
        }
    }

    public void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_dataKey, json);
    }
}

[System.Serializable]
public class GameData
{
    public int Score;
    public List<string> BoardData;
}