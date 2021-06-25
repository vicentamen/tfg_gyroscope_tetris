using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class controls the core game loop. Making pieces fall, reading the player input and checking the win/lose conditions
/// </summary>
public class GameLoopManager : MonoBehaviour
{
    [SerializeField] private CoreLoopData _gameLoopData;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayfieldManager _playfield;
    [SerializeField] private PieceManager _pieceManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private GameOverMenu _gameOverMenu;

    //Game state variables
    private bool _isGameOver = false;
    private bool _isPaused = false;
    public bool isGameOver { get => _isGameOver; }



    #region GAME_INITIALIZATION
    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    /// <summary>
    /// Initialixe the game and all the mage elements
    /// </summary>
    private void InitGame()
    {
        //Initialize playfield
        _playfield.Initialize();
        //Init pieces
        _pieceManager.Initialize(_gameLoopData.pieces, _playfield.gridData);
        //Initialize the player controller needs to be updated manually on the Update function
        _playerController.Initialize(_gameLoopData);
        _playerController.onPiecePlaced.AddListener(OnPiecePlaced);

        //Initialize Score manager
        _scoreManager.Initialize();

        _isPaused = false;
        _isGameOver = false;

        //Startgame
        StartGame();
    }

    private void StartGame()
    {
        _playerController.SetNewActivePiece(_pieceManager.GetNextPiece());
        _playerController.Enable();
    }
    #endregion

    #region UPDATE_GAME_STATE
    // Update is called once per frame
    void Update()
    {
        if (!_isGameOver && !_isPaused) //Will have to check the game state after each one on the loops have been tested
        {
            _playerController.UpdatePlayer();

        }
    }

    private void OnPiecePlaced(PieceBase placedPiece)
    {
        ScreenShake.DoBounce(Vector3.down);

        //Place piece
        PiecePlaceResult placeResult = _playfield.PlacePiece(placedPiece);
        _scoreManager.PiecePlaced();

        if(placeResult.completedCount > 0) //Lines have been cleared
        {
            _playfield.ClearLines(placeResult.completedLines);
            _scoreManager.LinesCleared(placeResult.completedCount);

            Vibrator.CreateWaveform(new long[] { 1000, 100, 200, 100 }, -1);
        }

        if (Playfield.IsBoardLinesFull())
        {
            GameOver();
        }
        else
        {
            _playerController.SetNewActivePiece(_pieceManager.GetNextPiece());
            _playerController.Enable();
        }
    }

    /// <summary>
    /// Pause the game and pause the input and fall loops
    /// </summary>
    private void PauseGame()
    {
        _isPaused = true;
        _playerController.Disable(); //Disable the player so the piece does not rotate or fall
    }

    /// <summary>
    /// Resume the game and play the Input and fall loops
    /// </summary>
    private void ResumeGame()
    {
        _playerController.Enable(); //Re enable the player to keep playing
        _isPaused = false;
    }

    private void GameOver()
    {
        _isGameOver = true;
        _playerController.Disable();

        Playfield.ClearBoard(() => _gameOverMenu.ShowMenu(_scoreManager.score));

        Vibrator.CreateOneShot(1500);
    }
    #endregion
}