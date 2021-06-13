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
    public Transform[] pieceBlocks { get => _pieceBlocks; }

    public void InitPiece(PlayfieldGrid playfield)
    {
        //Change piece size to fit the board size
        UpdatePieceSize(playfield.cellSize);
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
        if (playfield.IsOutOfBounds(new Vector3(x, y, 0)))
            movementResult = onMoveFailure;
        else
            movementResult = onMoveSuccess;

        y = Mathf.Clamp(y, -playfield.gridData.worldSizeY / 2f, playfield.gridData.worldSizeY / 2f); //The y is clamped later we need to know if the piece was going to fall outside the 


        //Move Piece
        transform.position = new Vector3(x, y, transform.position.z);
        //Execute movement result
        movementResult?.Invoke();
    }

    public virtual void RotatePiece(Vector2 moveDir)
    {
        //Check rotation
    }
    #endregion
}
