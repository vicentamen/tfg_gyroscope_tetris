using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MoveAttempt
{
    public Vector2 moveDir { get => _moveDir; }
    private Vector2 _moveDir;

    public Vector3 position { get => _position; }
    private Vector3 _position;

    public ROTATION_TYPE rotation { get => _rotation; }
    private ROTATION_TYPE _rotation;

    public PieceBase piece { get => _piece; }
    private PieceBase _piece;

    public MOVE_STATE moveState { get => _state; }
    private MOVE_STATE _state;

    public MoveAttempt(Vector2 moveDir, PieceBase piece)
    {
        _moveDir = moveDir;
        _piece = piece;

        _position = piece.transform.position;
        _rotation = ROTATION_TYPE.NONE;

        _state = MOVE_STATE.SUCCESS;

        AttemptMove();
    }

    private void AttemptMove()
    {
        if (Mathf.Abs(_moveDir.x) > 0)
            AttemptMoveHorizontal();

        if (_moveDir.y < 0)
            AttemptFall();
    }

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

    private void AttemptMoveLeft(float x)
    {
        int collisionsCounter = 0;
        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = 0; i < _piece.pieceGrid.GetLength(0); i++) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float offset = (piece.isPivotOffsetted) ? Playfield.gridData.cellSize / 2f : 0f;
                    float blockX = x + ((i - 1) * Playfield.gridData.cellSize) - offset;
                    float blockY = _position.y + ((j - 1) * Playfield.gridData.cellSize) - offset;
                    Vector2 newPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if (Playfield.IsPieceOutOfBounds(newPos))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return;
                    }
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(blockX, blockY))))
                    {
                        collisionsCounter++; //Increase the collision counter
                        /*if (collisionsCounter >= (_piece.pieceGrid.GetLength(1) / 2f)) //If the collision surface is bigger than the piece height then it cannot rotate
                        { 
                            _moveState = MOVE_STATE.SUCCESS; //The piece can at least keep fallings
                            return; //The position stays the same
                        }

                        return;*/
                    }

                }
            }
        }

        //Analyze collisions
        if(collisionsCounter > 0)
        {
            _state = MOVE_STATE.SUCCESS;
            return; //The piece stays in the same position
        }

        _position.x = x;
    }

    private void AttemptMoveRight(float x)
    {
        int collisionsCounter = 0;
        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = _piece.pieceGrid.GetLength(0) - 1; i >= 0; i--) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float offset = (piece.isPivotOffsetted) ? Playfield.gridData.cellSize / 2f : 0f;
                    float blockX = x + ((i - 1) * Playfield.gridData.cellSize) - offset;
                    float blockY = _position.y + ((j - 1) * Playfield.gridData.cellSize) - offset;
                    Vector2 newPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if (Playfield.IsPieceOutOfBounds(newPos))
                    {
                        _state = MOVE_STATE.SUCCESS;
                        return;
                    }
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(blockX, blockY))))
                    {
                        collisionsCounter++; //Increase the collision counter
                        /*if (collisionsCounter >= (_piece.pieceGrid.GetLength(1) / 2f)) //If the collision surface is bigger than the piece height then it cannot rotate
                        {
                            _moveState = MOVE_STATE.SUCCESS; //The piece can still keep falling
                            return; //The position stays the same
                        }*/
                    }
                }
            }
        }

        //Analyze collisions
        if (collisionsCounter > 0)
        {
            _state = MOVE_STATE.SUCCESS;
            return; //The piece stays in the same position
        }

        _position.x = x;
    }

    private void AttemptFall()
    {
        //Calculate new vertical position for the piece
        float y = _piece.transform.position.y + (Mathf.Clamp(moveDir.y, -1, 0) * Playfield.gridData.cellSize);

        //Check collisions on the playfield
        int collisionsCounter = 0;
        for (int j = 0; j < _piece.pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = _piece.pieceGrid.GetLength(0) - 1; i >= 0; i--) //From right to left
            {
                if (_piece.pieceGrid[i, j] != null)
                {
                    float offset = (piece.isPivotOffsetted) ? Playfield.gridData.cellSize / 2f : 0f;
                    float blockX = _position.x + ((i - 1) * Playfield.gridData.cellSize) - offset;
                    float blockY = y + ((j - 1) * Playfield.gridData.cellSize) - offset;
                    Vector2 newPos = new Vector2(blockX, blockY);
                    //Chekc if block will be out of bounds
                    if(Playfield.IsPieceOutOfBounds(newPos))
                    {
                        _state = MOVE_STATE.FAILURE;
                        return;
                    }

                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(newPos)))//There has been a collision
                    {
                        collisionsCounter++;
                        /*if (collisionsCounter >= _piece.pieceGrid.GetLength(0)) //If the collisions is bigger than half of the width then it cannot rotate or keep falling
                        {
                            _moveState = MOVE_STATE.FAILURE; //Cannot move anymore and so it has to be placed
                            return; //There is no need to keep checking for more
                        }

                        _moveState = MOVE_STATE.FAILURE;
                        return;*/
                    }
                }
            }
        }


        //Analyze collisions
        if (collisionsCounter > 0)
        {
            _state = MOVE_STATE.FAILURE;
            return; //The piece stays in the same position
        }

        _position.y = y;
    }
}

public enum MOVE_STATE
{
    SUCCESS, FAILURE
}

public enum ROTATION_TYPE
{
    NONE, LEFT, RIGHT
}
