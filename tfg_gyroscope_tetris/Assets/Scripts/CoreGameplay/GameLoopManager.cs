using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Timer _activePieceFallLoop; //Needs to be updated on the Update method

    //Game state variables
    private bool _isGameOver = false;
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
        //Initialize pieces pools
        //Initialize Playfield
        //Initialize Score manager
        //Initialize Loop timers
        _playerInputLoop = new Timer(_gameLoopData.timeBetweenInputs, UpdatePlayerInput);
        _activePieceFallLoop = new Timer(_gameLoopData.timeBetweenFall, MakePieceFall);

        _isGameOver = false;
    }
    #endregion

    #region UPDATE_GAME_STATE
    // Update is called once per frame
    void Update()
    {
        if (!_isGameOver) //Will have to check the game state after each one on the loops have been tested
        {
            _playerInputLoop.Update();
            _activePieceFallLoop.Update();
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
    private void MakePieceFall()
    {
        Debug.LogError("Piece falling every " + _gameLoopData.timeBetweenFall + " seconds");
        //Make piece fall
        //Update piece state -- Does the piece need to be rotated or not?
        //Check game state -- Is game over? -- has any line been completed? How many of them?
        //Update game state
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
