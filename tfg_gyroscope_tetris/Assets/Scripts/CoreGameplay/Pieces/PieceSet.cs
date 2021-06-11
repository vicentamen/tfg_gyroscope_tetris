using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GyroTetris/New Piece Set", fileName = "New Piece Set")]
public class PieceSet : ScriptableObject
{
    [SerializeField, Tooltip("Set of pieces that the player is going to play with")] 
    private List<GameObject> _pieces = new List<GameObject>();
    /// <summary>
    /// Set of pieces that the player is going to use to play 
    /// </summary>
    public List<GameObject> pieces { get => _pieces; }
}
