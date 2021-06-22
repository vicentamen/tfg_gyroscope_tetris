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
    [SerializeField] private bool _canRotate = true;
    /// <summary>
    /// Whether or not the blocks are offsetted from the piece pivot
    /// </summary>
    public bool isPivotOffsetted { get => _isPivotOffsetted; }
    public bool canRotate { get => _canRotate; }


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

    /// <summary>
    /// Dynamically creates the matrix grid that will contain all the blocks positions in the game logic
    /// </summary>
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
    /// <summary>
    /// Moves the piece to the new position
    /// </summary>
    /// <param name="newPos"></param>
    public void Move(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void Rotate(float rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        //Rotate grid
        if (rotation >= 90f && rotation < 180f)
            _rotatedGrid = GetRotatedGrid90();
        else if (rotation >= 180f && rotation < 270)
            _rotatedGrid = GetRotatedGrid180();
        else if (rotation >= 270 && rotation < 360)
            _rotatedGrid = GetRotatedGridMinus90();
        else
            _rotatedGrid = _pieceGrid;
    }

    /// <summary>
    /// Rotates the piece and grid 90 degrees
    /// </summary>
    public void RotateRight()
    {
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 90));

        _rotatedGrid = GetRotatedGrid90();
    }

    /// <summary>
    /// Rotates the piece and grid -90 degrees
    /// </summary>
    public void RotateLeft()
    {
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -90));

        _rotatedGrid = GetRotatedGridMinus90();
    }
    #endregion

    #region GET_PIECE_DATA
    /// <summary>
    /// Returns the current grid of the piece rotated -90 degrees
    /// </summary>
    public Transform[,] GetRotatedGridMinus90()
    {
        Transform[,] rotatedGrid = new Transform[_pieceGrid.GetLength(1), _pieceGrid.GetLength(0)];
        for (int i = 0; i < _pieceGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _pieceGrid.GetLength(1); j++)
            {
                int x = j;
                int y = (rotatedGrid.GetLength(1) - 1) - i;
                rotatedGrid[x, y] = _pieceGrid[i, j];
            }
        }

        return rotatedGrid;
    }

    /// <summary>
    /// Returns the current grid of the piece rotated 90 degrees
    /// </summary>
    public Transform[,] GetRotatedGrid90()
    {
        Transform[,] rotatedGrid = new Transform[_pieceGrid.GetLength(1), _pieceGrid.GetLength(0)];
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

    public Transform[,] GetRotatedGrid180()
    {
        Transform[,] rotatedGrid = new Transform[_pieceGrid.GetLength(0), _pieceGrid.GetLength(1)];
        for(int i = 0; i < rotatedGrid.GetLength(0); i++)
        {
            for(int j = 0; j < rotatedGrid.GetLength(1); j++)
            {
                int x = (_pieceGrid.GetLength(0) - 1) - i;
                int y = (_pieceGrid.GetLength(1) - 1) - j;
                rotatedGrid[x, y] = _pieceGrid[i, j];
            }
        }

        return rotatedGrid;
    }

    /// <summary>
    /// Returns the distance from the furthest left and bottom to the pivot
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMinRelative(Transform[,] gridReference)
    {
        float x = Mathf.Infinity;
        float y = Mathf.Infinity;
        for(int i = 0; i < gridReference.GetLength(0); i++)
        {
            for(int j = 0; j < gridReference.GetLength(1); j++)
            {
                if(gridReference[i,j] != null)
                {
                    if (gridReference[i,j].position.x < x)
                        x = gridReference[i, j].position.x;

                    if (gridReference[i, j].position.y < y)
                        y = gridReference[i, j].position.y;
                }
            }
        }

        return new Vector2(transform.position.x - x, transform.position.y - y);
    }

    /// <summary>
    /// Returns the distance from the furthest right and top to the pivot
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMaxRelative(Transform[,] gridReference)
    {
        float x = -Mathf.Infinity;
        float y = -Mathf.Infinity;
        for (int i = 0; i < gridReference.GetLength(0); i++)
        {
            for (int j = 0; j < gridReference.GetLength(1); j++)
            {
                if (gridReference[i, j] != null)
                {
                    if (gridReference[i, j].position.x > x)
                        x = gridReference[i, j].position.x;

                    if (gridReference[i, j].position.y > y)
                        y = gridReference[i, j].position.y;
                }
            }
        }

        return new Vector2(x - transform.position.x, y - transform.position.y);
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if(_pieceGrid != null)
        {
            for (int i = 0; i < pieceGrid.GetLength(0); i++) //From bot to top
            {
                for (int j = 0; j < pieceGrid.GetLength(1); j++) //From left to right
                {
                    if (pieceGrid[i, j] != null)
                    {
                        float x = transform.position.x + (i - ((pieceGrid.GetLength(0) / 2f) - (Playfield.gridData.cellSize / 2f)));
                        float y = transform.position.y + (j - ((pieceGrid.GetLength(0) / 2f) - (Playfield.gridData.cellSize / 2f)));
                        Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                    }
                }
            }
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
