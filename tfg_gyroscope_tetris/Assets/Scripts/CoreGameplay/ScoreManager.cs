using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreAndPlayerProgress _scoreData;
    [SerializeField] private TMP_Text _scoreText;
    public int score { get => _score; }
    private int _score;

    public void Initialize()
    {
        _score = 0;
        UpdateScore(0);
    }
    
    public void LinesCleared(int linesCount)
    {
        UpdateScore(_scoreData.LinesCleared(linesCount - 1));
    }

    public void PiecePlaced()
    {
        UpdateScore(_scoreData.PiecePlaced());
    }

    private void UpdateScore(int score)
    {
        _score += score;
        _scoreText.SetText(_score.ToString());
    }
}
