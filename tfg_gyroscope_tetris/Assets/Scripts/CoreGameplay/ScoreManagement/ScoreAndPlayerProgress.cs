using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = " GyroTetris/new Player progress")]
public class ScoreAndPlayerProgress : ScriptableObject
{
    [SerializeField] private int _scorePerPiece;
    [SerializeField] private int _scorePerLine;
    [SerializeField] List<float> _comboMultipliers;


    public int LinesCleared(int linesCount)
    {
        if (linesCount > _comboMultipliers.Count)
            linesCount = _comboMultipliers.Count;

        return Mathf.RoundToInt(_scorePerLine * _comboMultipliers[linesCount]);
    }

    public int PiecePlaced()
    {
        return _scorePerPiece;
    }
}
