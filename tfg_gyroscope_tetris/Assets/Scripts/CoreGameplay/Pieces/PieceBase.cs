using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBase : MonoBehaviour
{
    [Header("VISUALS PROPERTIES")]
    public Sprite blockSprite;
    public Sprite menuPreviewSprite;

    [Header("GAMEPLAY PROPERTIES")]
    [SerializeField] protected float _startingColumn;
    [SerializeField] protected float _startingLine;

    [SerializeField] private SpriteRenderer[] _pieceBlocks;
    public SpriteRenderer[] pieceBlocks { get => _pieceBlocks; }

    public void InitPiece()
    {
        //Change piece size to fit the board size
    }
}
