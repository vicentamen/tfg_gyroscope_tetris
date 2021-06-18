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
        if (Input.GetKeyDown(KeyCode.R)) //Only for testing purposes
            _activePiece.RotateLeft();

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
        MoveAttempt moveAttempt = new MoveAttempt(moveDir, _activePiece);

        _activePiece.Move(moveAttempt.position);
        _activePiece.Rotate(moveAttempt.rotation);

        return moveAttempt.moveState == MOVE_STATE.SUCCESS;
    }

    public void SetNewActivePiece(PieceBase newPiece)
    {
        newPiece.transform.position = Playfield.GetPieceSpawnWorldPosition(newPiece.isPivotOffsetted);
        newPiece.Enable();

        _activePiece = newPiece;
    }
    #endregion
}
