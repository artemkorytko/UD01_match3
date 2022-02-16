using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameScreen : MonoBehaviour
{
    [SerializeField] private Text _scoreText = null;

    public void ChangeScore(int score)
    {
        _scoreText.text = score.ToString();
    }
}
