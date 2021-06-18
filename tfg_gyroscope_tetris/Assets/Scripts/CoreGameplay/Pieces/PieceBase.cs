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
    private Transform[,] _rotatedGrid;
    public Transform[,] pieceGrid { get => _rotatedGrid; }
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
        int size = Mathf.Max(columns, rows);
        _pieceGrid = new Transform[size, size];

        //Distribute the pieces on the grid
        foreach(Transform block in _pieceBlocks) //Math taken from Playfield.FromWorldPointToGridPosition
        {
            float percentX = (block.position.x + size / 2f) / size;
            float percentY = (block.position.y + size / 2f) / size;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int i = Mathf.RoundToInt((size - 1) * percentX);
            int j = Mathf.RoundToInt((size - 1) * percentY);
            _pieceGrid[i, j] = block; 
        }

        _rotatedGrid = _pieceGrid;
    }

    public void Enable()
    {
        _rotatedGrid = _pieceGrid;
        transform.rotation = Quaternion.identity;

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

    public void Rotate(ROTATION_TYPE rotation)
    {
        switch (rotation)
        {
            case ROTATION_TYPE.RIGHT:
                RotateRight();
                break;
            case ROTATION_TYPE.LEFT: 
                RotateLeft();
                break;
            case ROTATION_TYPE.NONE:
            default:
                break;
        }
    }

    public void RotateRight()
    {
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 90));

        _rotatedGrid = GetRotatedGrid90();
    }

    public void RotateLeft()
    {
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -90));

        _rotatedGrid = GetRotatedGridMinus90();
    }

    private Transform[,] GetRotatedGridMinus90()
    {
        Transform[,] rotatedGrid = new Transform[pieceGrid.GetLength(1), pieceGrid.GetLength(0)];
        for (int i = 0; i < pieceGrid.GetLength(0); i++)
        {
            for (int j = 0; j < pieceGrid.GetLength(1); j++)
            {
                int x = j;
                int y = (rotatedGrid.GetLength(1) - 1) - i;
                rotatedGrid[x, y] = pieceGrid[i, j];
            }
        }

        return rotatedGrid;
    }

    private Transform[,] GetRotatedGrid90()
    {
        Transform[,] rotatedGrid = new Transform[pieceGrid.GetLength(1), pieceGrid.GetLength(0)];
        for (int i = 0; i < pieceGrid.GetLength(0); i++)
        {
            for (int j = 0; j < pieceGrid.GetLength(1); j++)
            {
                int x = (rotatedGrid.GetLength(0) - 1) - j;
                int y = i;
                rotatedGrid[x, y] = _pieceGrid[i, j];
            }
        }

        return rotatedGrid;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if(_pieceGrid != null)
        {
            /*Rect pieceRect = GetRect(transform.position);
            Gizmos.DrawCube(pieceRect.position, new Vector3(pieceRect.width, pieceRect.height));*/
            for (int i = 0; i < pieceGrid.GetLength(0); i++) //From bot to top
            {
                for (int j = 0; j < pieceGrid.GetLength(1); j++) //From left to right
                {
                    if (pieceGrid[i, j] != null)
                    {
                        float offset = (_isPivotOffsetted) ? Playfield.gridData.cellSize / 2f : 0f;
                        float x = transform.position.x + ((i - 1) * Playfield.gridData.cellSize) - offset;
                        float y = transform.position.y + ((j - 1) * Playfield.gridData.cellSize) - offset;
                        Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                    }
                }
            }

            Gizmos.color = Color.red;
            //Gizmos.DrawSphere(pieceCenter, 0.3f);
        }
    }

    private void OnValidate()
    {
        if (_pieceBlocks != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SpriteRenderer block;
                if (transform.GetChild(i).TryGetComponent(out block))
                {
                    if (block.sprite != blockSprite)
                    {
                        block.sprite = blockSprite;
                    }
                }
            }
        }
    }
}
