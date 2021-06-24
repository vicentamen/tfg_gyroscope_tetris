using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAttempt
{
    public PieceBase piece { get => _piece; }
    private PieceBase _piece;

    private SCREEN_ORIENTATION _targetOrientation;
    private Transform[,] _rotationGrid;

    public PIECE_ROTATION_STATE state { get => _rotationState; }
    private PIECE_ROTATION_STATE _rotationState;

    public Vector3 position { get => _position; }
    private Vector3 _position;
   
    public RotationAttempt (SCREEN_ORIENTATION targetOrientation, PieceBase rotatedPiece)
    {
        _targetOrientation = targetOrientation;
        _piece = rotatedPiece;

        float targetRotation = Rotator.GetRotationFromOrientation(_targetOrientation) - _piece.startRotation;
        if (targetOrientation < 0)
            targetRotation += 360f;
        else if (targetRotation > 360)
            targetRotation -= 360f;

        _rotationGrid = _piece.GetRotatedGrid(targetRotation);

        _position = _piece.transform.position;
        _rotationState = PIECE_ROTATION_STATE.SUCCESS;

        AttemptRotation();
    }

    private void AttemptRotation()
    {
        //move position to inside the board if it is outside
        Vector3 piecePosition = CheckPieceOutOfBounds(_position);
        //Check if the position is available
        if (CheckAvailablePosition(piecePosition)) //If the new position is available, then simply
        {
            _position = piecePosition;
            _rotationState = PIECE_ROTATION_STATE.SUCCESS;
            return;
        }

        //If we did not find any available position
        _rotationState = PIECE_ROTATION_STATE.FAILURE; // The piece cannot rotate
    }

    private Vector3 CheckPieceOutOfBounds(Vector3 pos)
    {
        int blocksOutRight = 0, blocksOutLeft = 0, blocksOutBottom = 0;
        //Check if the piece is out of bounds
        for (int i = 0; i < _rotationGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _rotationGrid.GetLength(1); j++)
            {
                //Calculate Block new position
                float blockX = pos.x + CalculateBlockWorldPosition(i, _rotationGrid.GetLength(0));
                float blockY = pos.y + CalculateBlockWorldPosition(j, _rotationGrid.GetLength(1));

                //Check if block is inside of bounds
                if (Playfield.IsBlockOutRight(blockX))
                    blocksOutRight++;
                else if (Playfield.IsBlockOutLeft(blockX))
                    blocksOutLeft++;
                if (Playfield.IsClockOutBottom(blockY))
                    blocksOutBottom++;
            }
        }

        if (blocksOutBottom == 0 && blocksOutLeft == 0 && blocksOutRight == 0) //if the piece is entirely inside the board
            return _position; //return the current position, there is no need for change

        //Move piece to inside the board
        float newX = pos.x - (blocksOutRight * Playfield.gridData.cellSize) + (blocksOutLeft * Playfield.gridData.cellSize);
        float newY = pos.y + (blocksOutBottom * Playfield.gridData.cellSize);

        return new Vector3(newX, newY, _position.z);
    }

    private bool CheckAvailablePosition(Vector3 pos)
    {
        for(int i = 0; i < _rotationGrid.GetLength(0); i++)
        {
            for(int j = 0; j < _rotationGrid.GetLength(1); j++)
            {
                float blockX = pos.x + CalculateBlockWorldPosition(i, _rotationGrid.GetLength(0));
                float blockY = pos.y + CalculateBlockWorldPosition(j, _rotationGrid.GetLength(1));
                Vector2 newPos = new Vector2(blockX, blockY);

                if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(newPos))) //if a node does not have a position the return false;
                    return false;
            }
        }

        return true;
    }

    private float CalculateBlockWorldPosition(int blockIndex, float arrayLength)
    {
        return blockIndex - ((arrayLength / 2f) - (Playfield.gridData.cellSize / 2f));
    }
}

public enum PIECE_ROTATION_STATE
{
    SUCCESS, FAILURE
}
