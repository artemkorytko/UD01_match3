using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private const string DATA_KEY = "Match3.PlayerData";
    private PlayerData _playerData = null;

    public PlayerData Data => _playerData;

    public void Load()
    {
        if (PlayerPrefs.HasKey(DATA_KEY))
        {
            string jsonData = PlayerPrefs.GetString(DATA_KEY);
            _playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            _playerData = new PlayerData();
        }
    }

    public void Save(PlayerData playerData)
    {
        _playerData = playerData;
        PlayerPrefs.SetString(DATA_KEY, JsonUtility.ToJson(_playerData));
    }
}

[System.Serializable]
public class PlayerData
{
    public int Score = 0;
    public List<string> BoardState = new List<string>();
}
