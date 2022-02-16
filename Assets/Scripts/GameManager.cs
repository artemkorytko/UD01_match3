using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _pointsForElement = 10;

    [Header("References")]
    [SerializeField] private SaveSystem _saveSystem = null;
    [SerializeField] private BoardController _boardController = null;
    [SerializeField] private UIGameScreen _gameScreen = null;

    private PlayerData _playerData = null;

    private void Awake()
    {
        _saveSystem.Load();
        _playerData = _saveSystem.Data;
        _gameScreen.ChangeScore(_playerData.Score);
        InitializeBoard();
    }

    public void ResetGame()
    {
        _playerData = new PlayerData();
        _saveSystem.Save(_playerData);
        _gameScreen.ChangeScore(_playerData.Score);
        _boardController.Reset();
    }

    private  void OnMatch(int count)
    {
        _playerData.Score += count * _pointsForElement;
        _gameScreen.ChangeScore(_playerData.Score);
    }

    private void InitializeBoard()
    {
        if (_playerData.BoardState.Count == 0)
        {
            _boardController.Initialize();
        }
        else
        {
            _boardController.Initialize(_playerData.BoardState);
        }

        _boardController.OnBoardClosed += SaveGameState;
        _boardController.OnMatch += OnMatch;
    }

    private void SaveGameState()
    {
        _playerData.BoardState = _boardController.GetBoardState();
        _saveSystem.Save(_playerData);
    }

    private void OnApplicationQuit()
    {
        SaveGameState();
    }
}
