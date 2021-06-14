using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class controls the core game loop. Making pieces fall, reading the player input and checking the win/lose conditions
/// </summary>
public class GameLoopManager : MonoBehaviour
{
    [Tooltip("Read the player input info. You can change it depending on the final build platform"), SerializeField]
    private InputSystem _inputSystem;
    [SerializeField]
    private CoreLoopData _gameLoopData;

    //Loop management variables
    private Timer _playerInputLoop; //Needs to be updated on the Update method
    private Timer _mainLoop; //Needs to be updated on the Update method

    //Game state variables
    private bool _isGameOver = false;
    private bool _isPaused = false;
    public bool isGameOver { get => _isGameOver; }

    //Game pieces and events management
    [SerializeField] private PieceManager _pieceManager;
    private PieceBase _activePiece; //The player active piece
    public Playfield playfield; //Will have to be private with a getter

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
        //Initialize Playfield
        playfield.Initialize();
        //Initialize pieces pools
        _pieceManager.Initialize(_gameLoopData.pieces, playfield.gridData);
        //Initialize Score manager
        //Initialize Loop timers
        _playerInputLoop = new Timer(_gameLoopData.timeBetweenInputs, UpdatePlayerInput); //Timers will require to be manually started from code
        _mainLoop = new Timer(_gameLoopData.timeBetweenFall, CorePieceLoop);

        _isPaused = false;
        _isGameOver = false;

        //Startgame
        StartGame();
    }

    private void StartGame()
    {
        //Get player a new active piece
        GetNewActivePiece();
        //Start loop timers
        _mainLoop.StartTimer();
       // _playerInputLoop.StartTimer();
    }
    #endregion

    #region UPDATE_GAME_STATE
    // Update is called once per frame
    void Update()
    {
        if (!_isGameOver && !_isPaused) //Will have to check the game state after each one on the loops have been tested
        {
            //_playerInputLoop.Update();
            _mainLoop.Update();
        }
    }

    /// <summary>
    /// Reads the player Input and checks and update the game state
    /// </summary>
    private void UpdatePlayerInput()
    {
        Debug.LogWarning("Reading player Input every" + _gameLoopData.timeBetweenInputs + " seconds");
        //Get Player Input
        //Update piece state -- Does the piece need to be rotated or not?
        //Check game state -- Is game over? -- has any line been completed? How many of them?
        //Update game state
    }

    /// <summary>
    /// Make the active piece fall one line and updates the game state
    /// </summary>
    private void CorePieceLoop()
    {
        //Move piece
        _activePiece.MovePiece(Vector2.down, playfield, _mainLoop.StartTimer, OnPiecePlaced);
            //Who is moving the piece? We said it shold be the piece itself
        //Is the piece placed?  --> The piece will need to return a true or false, or we can send two unityActions
            // ---> One UnityAction fow when the piece has been placed
                // -- Check the state of the board and if any lines have been filled
                // -- Add score if the line has been filled, we can give a callback to the playfield which will be the one to check the board's state
                // -- Check if it is game over or not, and either end the game or give the player a new active piece
            // ---> Other UnityAction for when the piece has moved correctly
            //If no, then continue with the fall loop
            //If yes, then get the player a new piece
    }

    private void OnPiecePlaced()
    {
        //Stop timers
        _mainLoop.StopTimer();
        _playerInputLoop.StopTimer();

        //Place Piece
        playfield.PlacePiece(_activePiece);

        _activePiece.gameObject.SetActive(false);
        //Add blocks to the playfield
        GetNewActivePiece();
        //RestartTimers
        _mainLoop.StartTimer();
        _playerInputLoop.StartTimer();
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

    #region PIECE_MANAGEMENT
    /// <summary>
    /// Gives the player a new active piece
    /// </summary>
    private void GetNewActivePiece()
    {
        _activePiece = _pieceManager.GetNextPiece();
        _activePiece.transform.position = playfield.GetPieceSpawnWorldPosition(_activePiece.isPivotOffsetted);
        _activePiece.gameObject.SetActive(true);
    }

    private void MoveActivePiece(Vector2 moveDirection, UnityAction onPiecePlaced)
    {
        //tell the active pice to move. The active piece will calculate the new position and if check for collisions and rotations
    }

    private void PlacePiece()
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
