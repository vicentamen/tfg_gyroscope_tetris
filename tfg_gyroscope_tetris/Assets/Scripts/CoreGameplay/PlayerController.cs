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

    public Transform screen;

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

        return moveAttempt.moveState == MOVE_STATE.SUCCESS;
    }

    public void SetNewActivePiece(PieceBase newPiece)
    {
        newPiece.transform.position = Playfield.GetPieceSpawnWorldPosition(newPiece.isPivotOffsetted);
        newPiece.Enable();

        _activePiece = newPiece;
    }

    public void RotateActivePiece(SCREEN_ORIENTATION orientation)
    {
        if(orientation == SCREEN_ORIENTATION.NONE) //The player is rotating so it has to pause input and fall loops
        {
            _inputReadTimer.PauseTimer();
            _pieceFallTimer.PauseTimer();
        }
        else
        {
            float rotation = Rotator.GetRotationFromOrientation(orientation);
            _activePiece.Rotate(rotation);
            screen.transform.DORotate(new Vector3(0, 0, Rotator.GetRotationFromOrientation(orientation)), 0.5f);

            //Should check if the rotation is acceptable

            _orientation = orientation;
            _inputReadTimer.ResumeTimer();
            _pieceFallTimer.ResumeTimer();
        }
    }
    #endregion
}
