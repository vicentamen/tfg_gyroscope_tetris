using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Game loop data", menuName = "GyroTetris/new game loop data")]
public class CoreLoopData : ScriptableObject
{
    [SerializeField, Tooltip("How much time in seconds does it take for the game to read the player Input, if less or equals than 0, the input will be read every frame")]
    private float _timeBetweenInputs = 0f;
    [SerializeField, Tooltip("How much time in seconds does it take for a piece to fall one line, if less or equals than 0, then the piece will fall every frame"), Min(0)]
    private float _timeBetweenFall = 1f;

    /// <summary>
    /// How much time in seconds does it take for the game to read the player Input, if 0, the input will be read every frame
    /// </summary>
    public float timeBetweenInputs { get => _timeBetweenInputs; }
    /// <summary>
    /// How much time in seconds does it take for a piece to fall one line, if 0, then the piece will fall every frame
    /// </summary>
    public float timeBetweenFall { get => _timeBetweenFall; }
}
