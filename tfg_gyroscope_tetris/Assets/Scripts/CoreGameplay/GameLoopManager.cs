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
    [SerializeField] private Playfield _playfield;
    [SerializeField] private PieceManager _pieceManager;

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
        //Place piece
        _playfield.PlacePiece(placedPiece);
        //Check game condition
        //Is game over, is there any line filled?
        //Give new active piece and enable player controller
        _playerController.SetNewActivePiece(_pieceManager.GetNextPiece());
        _playerController.Enable();
    }

    /// <summary>
    /// Pause the game and pause the input and fall loops
    /// </summary>
    private void PauseGame()
    {
        
    }

    /// <summary>
    /// Resume the game and play the Input and fall loops
    /// </summary>
    private void ResumeGame()
    {

    }
    #endregion

    /// <summary>
    /// Check if the game has been finished
    /// </summary>
    /// <returns></returns>
    private bool CheckGameOver()
    {
        return _isGameOver;
    }
}