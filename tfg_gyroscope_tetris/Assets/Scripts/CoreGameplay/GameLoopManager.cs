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

    //Game management variables
    private Coroutine _playerInputLoop;
    private Coroutine _activePieceFallLoop;
    private bool _isGameOver = false;
    public bool isGameOver { get => _isGameOver; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Initialixe the game and all the mage elements
    /// </summary>
    private void InitGame()
    {
        //Initialize pieces pools
        //Initialize Playfield
        //Initialize Score manager

        _isGameOver = false;
    }

    /// <summary>
    /// Check if the game has been finished
    /// </summary>
    /// <returns></returns>
    private bool CheckGameOver()
    {
        return _isGameOver;
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
    //Build a tick Coroutine that will work as to read the input and make the pieces fall.
     //- This loops can work on the usual Update frequency or with custom times. 

}
