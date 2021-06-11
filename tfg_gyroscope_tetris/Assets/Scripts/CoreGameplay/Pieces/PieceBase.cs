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

    public void InitPiece(PlayfieldGrid playfield)
    {
        //Change piece size to fit the board size
        UpdatePieceSize(playfield.cellSize);
    }

    private void UpdatePieceSize(float cellSize)
    {
        transform.localScale = Vector3.one * cellSize; //Every block has 1 unit size so multiplying by the cell size should be enough to scale them to the right size for the grid
    }

    private void ResetPieceSize()
    {
        transform.localScale = Vector3.one; //Default size is the scale of one
    }
}
