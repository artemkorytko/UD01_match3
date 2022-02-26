using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int PointForElement = 10;

    private SaveSystem _saveSystem;
    private BoardController _boardController;
    
    public Action<int> OnScoreChange;

    private GameData _gameData;


    private void Awake()
    {
        _saveSystem = GetComponent<SaveSystem>();
        _boardController = FindObjectOfType<BoardController>();
        _saveSystem.Initialize();
        _gameData = _saveSystem.GameData;
    }

    private void Start()
    {
        _boardController.CreateGame(_gameData.BoardData);
        _boardController.OnMatch += OnMatch;
        OnScoreChange?.Invoke(_gameData.Score);
    }

    private void OnMatch(int value)
    {
        var sum = value * PointForElement;
        _gameData.Score += sum;
        OnScoreChange?.Invoke(_gameData.Score);
    }

    private void OnDestroy()
    {
        _boardController.OnMatch -= OnMatch;
    }

    public void Restart()
    {
        _gameData = new GameData();
        _saveSystem.SaveData(_gameData);
        OnScoreChange?.Invoke(_gameData.Score);
        _boardController.Reset();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void SaveGameData()
    {
        _gameData.BoardData = _boardController.GetBoardData();
        _saveSystem.SaveData(_gameData);
    }
}
