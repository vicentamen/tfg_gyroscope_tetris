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
    //Why should the piece be controlling the movement and rotation? because they hold the info
    //Because each piece can hold the special rules that apply to them, otherwise the loop manager, piece manager or playfield may become way to complex
    /// <summary>
    /// Move the piece in the desired direction
    /// </summary>
    /// <param name="moveDir">Vector with the direction of the movement</param>
    /// <param name="playfield">Reference to the playfield manager</param>
    /// <param name="onMoveSuccess">What to do if the piece has been successfully moved</param>
    /// <param name="onMoveFailure">What to do if the piece has no moved succcessfuly (has been placed)</param>
    public virtual void MovePiece(Vector2 moveDir, Playfield playfield, UnityAction onMoveSuccess, UnityAction onMoveFailure)
    {
        UnityAction movementResult;
        //Move piece
        //Calculate the new piece position
        float x = transform.position.x + (Mathf.Clamp(moveDir.x, -1, 1) * playfield.gridData.cellSize); //current position + (clamped Input * grid cellsize)
        x = Mathf.Clamp(x, -playfield.gridData.worldSizeX / 2f, playfield.gridData.worldSizeX / 2f); //Clamp to the borders of the screen

        float y = transform.position.y + (Mathf.Clamp(moveDir.y, -1, 0) * playfield.gridData.cellSize);
        //Check collisions
        //Check if the piece was going to fall outside the screen and place it if true
        if (HasCollidedAndBeenPlaced(new Vector3(x, y, 0), moveDir, playfield))
            movementResult = onMoveFailure;
        else
            movementResult = onMoveSuccess;

        y = Mathf.Clamp(y, -playfield.gridData.worldSizeY / 2f, playfield.gridData.worldSizeY / 2f); //The y is clamped later we need to know if the piece was going to fall outside the 


        //Move Piece
        transform.position = new Vector3(x, y, transform.position.z);
        //Execute movement result
        movementResult?.Invoke();
    }

    private bool HasCollidedAndBeenPlaced(Vector3 pos, Vector2 moveDir, Playfield playfield)
    {
        if (playfield.IsOutOfBounds(pos)) //Check if the piece has arrived to the lowe side of the screen
            return true;

        //Check collisions of the board -- Depending on the rotation and direction the piece should start the check in one direction or another
        //If Has to rotate, rotate it and return false
        //If it has to be placed return true
        if (moveDir.y < 0) //Going downwards
        {
            for(int i = 0; i < _pieceGrid.GetLength(0); i++)
            {
                for(int j = 0; j < _pieceGrid.GetLength(1); j++)
                {
                    if(_pieceGrid[i, j] != null)
                    {
                        //Calculate new block position
                        float x = pos.x + ((i - 1) * playfield.gridData.cellSize);
                        float y = pos.y + ((j - 1) * playfield.gridData.cellSize);
                        if (!playfield.IsNodeEmpty(playfield.FromWorldToGridPosition(new Vector2(x, y))))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public virtual void RotatePiece(Vector2 moveDir)
    {
        //Check rotation
    }
    #endregion
}
