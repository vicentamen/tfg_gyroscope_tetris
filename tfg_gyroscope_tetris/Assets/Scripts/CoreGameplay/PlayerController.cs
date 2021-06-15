using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("Platform input system for the player")] 
    private InputSystem _inputSystem;

    private Timer _inputReadTimer;
    private Timer _pieceFallTimer;

    [HideInInspector] public UnityEvent<PieceBase> onPiecePlaced;
    private PieceBase _activePiece;

    // Start is called before the first frame update
    void Start()
    {
        onPiecePlaced = new UnityEvent<PieceBase>();
    }

    public void Initialize(CoreLoopData gameData)
    {
        _inputReadTimer = new Timer(gameData.timeBetweenInputs, OnPlayerInputTimer);
        _pieceFallTimer = new Timer(gameData.timeBetweenFall, OnPieceFallTimer);
    }

    public void Enable()
    {
        _inputReadTimer.StartTimer();
        _pieceFallTimer.StartTimer();
    }

    #region LOOP_MANAGEMENT
    public void UpdatePlayer()
    {
        _inputReadTimer.Update();
        _pieceFallTimer.Update();
    }

    private void OnPlayerInputTimer()
    {
        if (MoveActivePiece(new Vector2(_inputSystem.GetHorizontal(), _inputSystem.GetVertical())))
            _inputReadTimer.StartTimer(); //Restart timer if it can keep moving
        else
            OnPiecePlaced();
          
    }

    private void OnPieceFallTimer()
    {
        if (MoveActivePiece(Vector2.down))
            _pieceFallTimer.StartTimer();
        else
            OnPiecePlaced();
    }

    private void OnPiecePlaced()
    {
        //Stop timers
        _inputReadTimer.StopTimer();
        _pieceFallTimer.StopTimer();

        //Disable active piece
        _activePiece.Disable();

        onPiecePlaced.Invoke(_activePiece);
    }
    #endregion
    #region PIECE_MANAGEMENT
    private bool MoveActivePiece(Vector2 moveDir)
    {
        //Calculate new piece position
        float x = _activePiece.transform.position.x + (Mathf.Clamp(moveDir.x, -1, 1) * Playfield.gridData.cellSize);
        float y = _activePiece.transform.position.y + (Mathf.Clamp(moveDir.y, -1, 0) * Playfield.gridData.cellSize);

        //Check if the piece is out of borders
        if (Playfield.IsPieceOutOfBounds(_activePiece.GetRect(new Vector2(x, _activePiece.transform.position.y))))
            x = _activePiece.transform.position.x;
        if (moveDir.y < 0 && Playfield.IsPieceOutOfBounds(_activePiece.GetRect(new Vector2(_activePiece.transform.position.x, y))))
            return false; //Cannot move and has to be placed

        //Check piece collisions
        if (moveDir.x < 0 && CheckCollisionLeft(new Vector2(x, _activePiece.transform.position.y), _activePiece.pieceGrid))
            x = _activePiece.transform.position.x;
        else if (moveDir.x > 0 && CheckCollisionRight(new Vector2(x, _activePiece.transform.position.y), _activePiece.pieceGrid))
            x = _activePiece.transform.position.x;

        if (moveDir.y < 0 && CheckCollisionDown(new Vector2(_activePiece.transform.position.x, y), _activePiece.pieceGrid))
            return false; //If there is collision place piece SHOULD CHECK ROTATIONS FIRST

        _activePiece.Move(new Vector3(x, y, _activePiece.transform.position.z)); //Move piece to new position
        return true; //The movement has been a success
    }

    public void SetNewActivePiece(PieceBase newPiece)
    {
        newPiece.transform.position = Playfield.GetPieceSpawnWorldPosition(newPiece.isPivotOffsetted);
        newPiece.Enable();

        _activePiece = newPiece;
    }
    #region PIECE_COLLISIONS_CHECKS
    private bool CheckCollisionDown(Vector3 piecePos, Transform[,] pieceGrid)
    {
        for (int i = 0; i < pieceGrid.GetLength(0); i++) //From left to right
        {
            for (int j = 0; j < pieceGrid.GetLength(1); j++) //From bot to top
            {
                if (pieceGrid[i, j] != null)//The position is not empty
                {
                    float x = piecePos.x + ((i - 1) * Playfield.gridData.cellSize);
                    float y = piecePos.y + (j * Playfield.gridData.cellSize);
                    if(!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(x, y))))
                    {
                        //Check possible rotations
                        return true; //There is no possible rotation and the piece has to be placed
                    }
                }
            }
        }

        return false; //The piece is not colliding
    }

    private bool CheckCollisionLeft(Vector3 piecePos, Transform[,] pieceGrid)
    {
        for(int j = 0; j < pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = 0; i < pieceGrid.GetLength(0); i++) //From left to right
            {
                if(pieceGrid[i, j] != null)
                {
                    float x = piecePos.x + ((i - 1) * Playfield.gridData.cellSize);
                    float y = piecePos.y + (j * Playfield.gridData.cellSize);
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(x, y))))
                    {
                        return true; //There is no possible rotation and the piece has to be placed
                    }
                }
            }
        }
        return false; //The piece is no colliding
    }

    private bool CheckCollisionRight(Vector3 piecePos, Transform[,] pieceGrid)
    {
        for (int j = 0; j < pieceGrid.GetLength(1); j++) //From bot to top
        {
            for (int i = pieceGrid.GetLength(0) - 1; i >= 0; i--) //From right to left
            {
                if (pieceGrid[i, j] != null)
                {
                    float x = piecePos.x + ((i - 1) * Playfield.gridData.cellSize);
                    float y = piecePos.y + (j * Playfield.gridData.cellSize);
                    if (!Playfield.IsNodeEmpty(Playfield.FromWorldToGridPosition(new Vector2(x, y))))
                    {
                        return true; //There is no possible rotation and the piece has to be placed
                    }
                }
            }
        }
        return false; //The piece is not colliding
    }
    #endregion
    #endregion
}
