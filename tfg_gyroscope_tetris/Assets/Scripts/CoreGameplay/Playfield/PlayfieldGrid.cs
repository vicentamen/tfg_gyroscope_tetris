using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GyroTetris/New Playfield Grid", fileName = "new Playfield Grid")]
public class PlayfieldGrid : ScriptableObject
{
    [Header("Size of the GRID in world coordinates")]
    [SerializeField] private float _worldSizeX = 13f;
    [SerializeField] private float _worldSizeY = 24f;
    public float worldSizeX { get => _worldSizeX; }
    public float worldSizeY { get => _worldSizeY; }

    [Header("How many blocks fit horizontal and vertically")]
    [SerializeField] private float _columns = 13f;
    [SerializeField] private float _rows = 24f;
    public float columns { get => _columns; }
    public float rows { get => _rows; }
    
    [Header("How big is each block")]
    [SerializeField] private float _cellSize;
    public float cellSize { get => _cellSize; }


    private void OnValidate()
    {
        _cellSize = _worldSizeX / _columns;
    }
}

