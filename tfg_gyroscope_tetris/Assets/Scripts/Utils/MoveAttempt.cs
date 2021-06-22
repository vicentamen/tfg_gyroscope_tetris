using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MoveAttempt
{
    public Vector2 moveDir { get => _moveDir; }
    private Vector2 _moveDir;

    public Vector3 position { get => _position; }
    private Vector3 _position;

    public ROTATION_DIRECTION rotation { get => _rotation; }
    private ROTATION_DIRECTION _rotation;

    public PieceBase piece { get => _piece; }
    private PieceBase _piece;

    public MOVE_STATE moveState { get => _state; }
    private MOVE_STATE _state;

    public MoveAttempt(Vector2 moveDir, PieceBase piece)
    {
        _moveDir = moveDir;
        _piece = piece;

        _position = piece.transform.position;
        _rotation = ROTATION_DIRECTION.NONE;

        _state = MOVE_STATE.SUCCESS;

        AttemptMove();
    }

    /// <summary>
    /// Attempts a move for the piece in the direction delivered to the object
    /// </summary>
    private void AttemptMove()
    {
        if (Mathf.Abs(_moveDir.x) > 0)
            AttemptMoveHorizontal();

        if (_moveDir.y < 0)
            AttemptFall();
    }

    /// <summary>
    /// Try to move the piece either left or right
    /// </summary>
    private void AttemptMoveHorizontal()
    {
        //Calculate new piece position
        float x = _piece.transform.position.x + (Mathf.Clamp(_moveDir.x, -1, 1) * Playfield.gridData.cellSize);

        //Check for collisions on the movement direction
        if (_moveDir.x < 0)
            AttemptMoveLeft(x);
        else if (_moveDir.x > 0)
            AttemptMoveRight(x);
    }

    /// <summary>
    /// Tries to move the piece to the left, and rotates it in case there is a collision
    /// </summary>
    /// <param name="x"></param>
    private void AttemptMoveLeft(float x)
    {
        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = 0; i < _piece.pieceGrid.GetLength(0); i++) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float blockX = x + CalculateBlockWorldPosition(i, _piece.pieceGrid.GetLength(0));
                    float blockY = _position.y + CalculateBlockWorldPosition(j, _piece.pieceGrid.GetLength(1));
                    Vector2 newPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if (Playfield.IsBlockOutOfBounds(newPos))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return;
                    }
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(blockX, blockY))))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return; //The piece stays in the same position/
                    }

                }
            }
        }

        _position.x = x;
    }

    private void AttemptMoveRight(float x)
    {
        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = _piece.pieceGrid.GetLength(0) - 1; i >= 0; i--) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float blockX = x + CalculateBlockWorldPosition(i, _piece.pieceGrid.GetLength(0));
                    float blockY = _position.y + CalculateBlockWorldPosition(j, _piece.pieceGrid.GetLength(1));
                    Vector2 newPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if (Playfield.IsBlockOutOfBounds(newPos))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return;
                    }
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(blockX, blockY))))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return; //The piece stays in the same position
                    }
                }
            }
        }

        _position.x = x;
    }

    private void AttemptFall()
    {
        //Calculate new vertical position for the piece
        float y = _piece.transform.position.y + (Mathf.Clamp(moveDir.y, -1, 0) * Playfield.gridData.cellSize);

        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = _piece.pieceGrid.GetLength(0) - 1; i >= 0; i--) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float blockX = _position.x + CalculateBlockWorldPosition(i, _piece.pieceGrid.GetLength(0));
                    float blockY = y + CalculateBlockWorldPosition(j, _piece.pieceGrid.GetLength(1));
                    Vector2 blockPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if(Playfield.IsBlockOutOfBounds(blockPos))
                    {
                        _state = MOVE_STATE.FAILURE;
                        return;
                    }

                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(blockPos)))//A collision has happend
                    {
                        _position = _piece.transform.position;
                        _state = MOVE_STATE.FAILURE;
                        return;
                    }
                }
            }
        }

        _position.y = y;
    }

    private float CalculateBlockWorldPosition(int blockIndex, float arrayLength)
    {
        return blockIndex - ((arrayLength / 2f) - (Playfield.gridData.cellSize / 2f));
    }
}

public enum MOVE_STATE
{
    SUCCESS, FAILURE
}

public enum ROTATION_DIRECTION
{
    NONE, LEFT, RIGHT
}

