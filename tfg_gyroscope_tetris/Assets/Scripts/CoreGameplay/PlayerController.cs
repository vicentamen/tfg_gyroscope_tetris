using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("Platform input system for the player")] 
    private InputSystem _inputSystem;
    [SerializeField] private RotationManager _rotManager;

    private Timer _inputReadTimer;
    private Timer _pieceFallTimer;

    [HideInInspector] public UnityEvent<PieceBase> onPiecePlaced;
    private PieceBase _activePiece;

    private SCREEN_ORIENTATION _orientation;

    // Start is called before the first frame update
    void Start()
    {
        onPiecePlaced = new UnityEvent<PieceBase>();
    }

    public void Initialize(CoreLoopData gameData)
    {
        _inputReadTimer = new Timer(gameData.timeBetweenInputs, OnPlayerInputTimer);
        _pieceFallTimer = new Timer(gameData.timeBetweenFall, OnPieceFallTimer);

        //Add listeners
        _rotManager.onRotation.AddListener(RotateActivePiece);
    }

    public void Enable()
    {
        _inputReadTimer.StartTimer();
        _pieceFallTimer.StartTimer();
    }

    public void Disable()
    {
        _rotManager.onRotation.RemoveListener(RotateActivePiece);
    }

    private void OnDisable()
    {
        Disable();
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
        float x = 0f;
        if (_orientation == SCREEN_ORIENTATION.PORTRAIT) x = _inputSystem.GetHorizontal();
        else if (_orientation == SCREEN_ORIENTATION.LANDSCAPE) x = -_inputSystem.GetVertical();
        else if (_orientation == SCREEN_ORIENTATION.INV_PORTRAIT) x = -_inputSystem.GetHorizontal();
        else if (_orientation == SCREEN_ORIENTATION.INV_LANDSCAPE) x = _inputSystem.GetVertical();

        if (MoveActivePiece(new Vector2(x, 0f)))
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

        if (Mathf.Abs(moveDir.x) > 0)
            Vibrator.CreateOneShot(50, 50);

        return moveAttempt.moveState == MOVE_STATE.SUCCESS;
    }

    public void SetNewActivePiece(PieceBase newPiece)
    {
        newPiece.transform.position = Playfield.GetPieceSpawnWorldPosition(newPiece.isPivotOffsetted);
        newPiece.Enable(Rotator.GetRotationFromOrientation(_orientation));

        _activePiece = newPiece;
    }

    public void RotateActivePiece(SCREEN_ORIENTATION orientation)
    {
        if(orientation != _orientation)
        {
            //Pause timers during rotation
            _inputReadTimer.PauseTimer();
            _pieceFallTimer.PauseTimer();

            //Check if the rotation is available
            RotationAttempt rotAttempt = new RotationAttempt(orientation, _activePiece);
            if(rotAttempt.state == PIECE_ROTATION_STATE.SUCCESS) //if the piece can freely rotate, then rotate
            {
                float rotation = Rotator.GetRotationFromOrientation(orientation);

                _activePiece.Move(rotAttempt.position);
                _activePiece.Rotate(rotation);
            }
            else //if the piece cannot rotate
            {
                //Give the piece the new starting rotation
            }

            _orientation = orientation;

            //delay turning on the timers a little bit
            DOVirtual.DelayedCall(1f, () =>
            {
                _inputReadTimer.ResumeTimer();
                _pieceFallTimer.ResumeTimer();
            });
        }
    }
    #endregion
}
