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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
