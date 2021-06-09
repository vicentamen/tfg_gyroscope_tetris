using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Piece", menuName = "GyroTetris/New Piece Data")]
public class PieceData : ScriptableObject
{
    [SerializeField] private GameObject _piecePrefab;
    [SerializeField] private Sprite _blockSprite;
    [SerializeField] private Sprite _menuPreviewSprite;

    public Sprite blockSprite { get => _blockSprite; }
    public Sprite previewSprite { get => _menuPreviewSprite; }
}
