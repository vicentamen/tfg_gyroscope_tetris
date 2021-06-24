using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreAndPlayerProgress _scoreData;
    [SerializeField] private TMP_Text _scoreText;
    private float _score;

    public void Initialize()
    {
        _score = 0f;
        UpdateScore(0f);
    }
    
    public void LinesCleared(int linesCount)
    {
        UpdateScore(_scoreData.LinesCleared(linesCount - 1));
    }

    public void PiecePlaced()
    {
        UpdateScore(_scoreData.PiecePlaced());
    }

    private void UpdateScore(float score)
    {
        _score += score;
        _scoreText.SetText(_score.ToString());
    }
}
