using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = " GyroTetris/new Player progress")]
public class ScoreAndPlayerProgress : ScriptableObject
{
    [SerializeField] private float _scorePerPiece;
    [SerializeField] private float _scorePerLine;
    [SerializeField] List<float> _comboMultipliers;


    public float LinesCleared(int linesCount)
    {
        if (linesCount > _comboMultipliers.Count)
            linesCount = _comboMultipliers.Count;

        return _scorePerLine * _comboMultipliers[linesCount];
    }

    public float PiecePlaced()
    {
        return _scorePerPiece;
    }
}
