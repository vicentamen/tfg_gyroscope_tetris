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
        float x = _activePiece.transform.position.x + (Mathf.Clamp(moveDir.x, -1, 1) * PlayfieldBoard.gridData.cellSize);
        float y = _activePiece.transform.position.y + (Mathf.Clamp(moveDir.y, -1, 0) * PlayfieldBoard.gridData.cellSize);

        //Check if the piece is out of borders
        if (PlayfieldBoard.IsPieceOutOfBounds(_activePiece.GetRect(new Vector2(x, _activePiece.transform.position.y))))
            x = _activePiece.transform.position.x;
        if (moveDir.y < 0 && PlayfieldBoard.IsPieceOutOfBounds(_activePiece.GetRect(new Vector2(_activePiece.transform.position.x, y))))
            return false; //Cannot move and has to be placed

        _activePiece.Move(new Vector3(x, y, _activePiece.transform.position.z)); //Move piece to new position
        return true; //The movement has been a success
    }

    public void SetNewActivePiece(PieceBase newPiece)
    {
        newPiece.transform.position = PlayfieldBoard.GetPieceSpawnWorldPosition(newPiece.isPivotOffsetted);
        newPiece.Enable();

        _activePiece = newPiece;
    }
    #endregion
}
