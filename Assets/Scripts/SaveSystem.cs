using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    private const string DataKey = "GameData";
    
    private GameData _gameData;

    public GameData GameData => _gameData;
    
    public void Initialize()
    {
        if (PlayerPrefs.HasKey(DataKey))
        {
            LoadData();
        }
        else
        {
            _gameData = new GameData();
        }
    }

    private void LoadData()
    {
        var jsonData = PlayerPrefs.GetString(DataKey);
        _gameData = JsonUtility.FromJson<GameData>(jsonData);
    }

    public void SaveData(GameData gameData)
    {
        var jsonData = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(DataKey, jsonData);
    }
}

[Serializable]
public class GameData
{
    public int Score;
    public List<string> BoardData;
}
