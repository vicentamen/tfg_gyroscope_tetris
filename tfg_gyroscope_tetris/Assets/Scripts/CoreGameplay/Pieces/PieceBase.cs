using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PieceBase : MonoBehaviour
{
    [Header("VISUALS PROPERTIES")]
    public Sprite blockSprite;
    public Sprite menuPreviewSprite;

    [Header("GAMEPLAY PROPERTIES")]
    [SerializeField, Tooltip("Wheter or not the blocks are offsetted from the piece pivot")] 
    private bool _isPivotOffsetted = false;
    /// <summary>
    /// Whether or not the blocks are offsetted from the piece pivot
    /// </summary>
    public bool isPivotOffsetted { get => _isPivotOffsetted; }


    [SerializeField] private Transform[] _pieceBlocks;
    private Transform[,] _pieceGrid; //Grid of the pieces blocks as they would appear in the playfield board the grid gets rotated
    public Transform[,] pieceGrid { get => GetPieceGrid(); }
    public Transform[] pieceBlocks { get => _pieceBlocks; }

    public void InitPiece(PlayfieldGrid playfield)
    {
        BuildPieceGrid();
        //Change piece size to fit the board size
        UpdatePieceSize(playfield.cellSize);
    }

    private void BuildPieceGrid()
    {
        //Create the grid
        float maxX = -Mathf.Infinity;
        float minX = Mathf.Infinity;

        float maxY = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        foreach(Transform block in _pieceBlocks)
        {
            if (block.position.x > maxX)
                maxX = block.transform.position.x;
            else if (block.position.x < minX)
                minX = block.transform.position.x;

            if (block.position.y > maxY)
                maxY = block.position.y;
            else if (block.position.y < minY)
                minY = block.transform.position.y;
        }

        int columns = Mathf.RoundToInt(maxX - minX) + 1; //We have to add the cell size to the substraction
        int rows = Mathf.RoundToInt(maxY - minY) + 1;
        _pieceGrid = new Transform[columns, rows];

        //Distribute the pieces on the grid
        foreach(Transform block in _pieceBlocks) //Math taken from Playfield.FromWorldPointToGridPosition
        {
            float percentX = (block.position.x + columns / 2f) / columns;
            float percentY = (block.position.y + rows / 2f) / rows;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int i = Mathf.RoundToInt((columns - 1) * percentX);
            int j = Mathf.RoundToInt((rows - 1) * percentY);
            _pieceGrid[i, j] = block; 
        }
    }

    public void Enable()
    {
        //reset the piece rotation here
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        //animate piece somewhere in here
        gameObject.SetActive(false);
    }

    #region PIECE_APPEAREANCE
    private void UpdatePieceSize(float cellSize)
    {
        transform.localScale = Vector3.one * cellSize; //Every block has 1 unit size so multiplying by the cell size should be enough to scale them to the right size for the grid
    }

    private void ResetPieceSize()
    {
        transform.localScale = Vector3.one; //Default size is the scale of one
    }
    #endregion

    #region MOVEMENT_&_ROTATION_MANAGEMENT
    public void Move(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void Rotate(Vector3 newRotation)
    {
        transform.rotation = Quaternion.Euler(newRotation);
    }
    
   private bool CheckRotation(Vector3 pos, PlayfieldManager playfield)
    {
        for (int i = 0; i < _pieceGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _pieceGrid.GetLength(1); j++)
            {
                if (_pieceGrid[i, j] != null)
                {
                    //Calculate new block position
                    float x = pos.x + ((i - 1) * playfield.gridData.cellSize);
                    float y = pos.y + ((j - 1) * playfield.gridData.cellSize);
                    if (!playfield.IsNodeEmpty(playfield.FromWorldToGridPosition(new Vector2(x, y))))
                    {
                        return false; //There is no possible rotation and so the piece has to be placed
                    }
                }
            }
        }

        return true;
    }

    public Rect GetRect(Vector3 rectPosition)
    {
        float width = _pieceGrid.GetLength(0) * transform.localScale.x;
        float height = _pieceGrid.GetLength(1) * transform.localScale.y;

        Vector3 pos = rectPosition + (Vector3.up * (transform.localScale.y / 2f)); //Offset the rect to match the pivot
        return new Rect(pos, new Vector2(width, height)); 
    }

    private Transform[,] GetPieceGrid()
    {
        //Create the piece grid rotated
        return _pieceGrid;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if(_pieceGrid != null)
        {
            /*Rect pieceRect = GetRect(transform.position);
            Gizmos.DrawCube(pieceRect.position, new Vector3(pieceRect.width, pieceRect.height));*/
            for (int j = 0; j < pieceGrid.GetLength(1); j++) //From bot to top
            {
                for (int i = 0; i < pieceGrid.GetLength(0); i++) //From left to right
                {
                    if (pieceGrid[i, j] != null)
                    {
                        float x = transform.position.x + ((i - 1) * Playfield.gridData.cellSize);
                        float y = transform.position.y + (j * Playfield.gridData.cellSize);
                        Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                    }
                }
            }
        }
    }
}
