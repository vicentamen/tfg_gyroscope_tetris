using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldManager : MonoBehaviour
{
    [SerializeField, Tooltip("Playfield grid data")]
    private PlayfieldGrid _gridData = null;

    [Header("BLOCK PROPERTIES")]
    [SerializeField] private Pool _blocksPool;
    [SerializeField, Tooltip("Reference to the container for the blocks gameobject placed on the playfield")]
    private Transform _blocksContainer;

    public PlayfieldGrid gridData { get => _gridData; }

    //Gotta have to save the grid state somewhere and the blocks linked to the positions
    private Block[,] _board; //It probably needs to be a block matrix. =========== WILL NEED TO FIND A WAY TO SAVE THE OBSTACLES POSITION IN THE BOARD ==============\\

    //Piece placement management
    private Vector2Int _pieceSpawnGridPosition;

    public void Initialize()
    {
        _board = new Block[_gridData.columns, _gridData.rows];
        _pieceSpawnGridPosition = new Vector2Int(Mathf.RoundToInt(_gridData.columns / 2), (_gridData.rows < 26) ? (_gridData.rows - 2) : 24);

        Playfield.SetupGameboard(this);
    }

    public Vector3 GetPieceSpawnWorldPosition(bool isPivotOffsetted)
    {
        //The result should change depending on wheter the pivot is offsetted or not
        float offset = (isPivotOffsetted) ? _gridData.cellSize / 2f : 0f;
        return FromGridToWorldPosition(_pieceSpawnGridPosition) - Vector3.one * offset;
    }

    public void PlacePiece(PieceBase piece)
    {
        //get the piece blocks position and add them to the board, will have to use BLOCK objects or not, maybe I can simply have SpriteReder-s and animate them later with DoTween
        foreach(Transform pieceBlock in piece.pieceBlocks)
        {
            Block newBlock = _blocksPool.GetItem().GetComponent<Block>();
            newBlock.PlaceBlock(pieceBlock.position, Vector3.one * _gridData.cellSize, piece);

            //Save the block in the board
            Vector2Int index = FromWorldToGridPosition(pieceBlock.position);
            _board[index.x, index.y] = newBlock;
        }
    }


    #region POSITION_CONVERTIONS
    public Vector2Int FromWorldToGridPosition(Vector2 worldPos) //Math taken from pathfinfing video of Sbastian Lague https://www.youtube.com/watch?v=nhiFx28e7JY&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=2
    {
        float percentX = (worldPos.x + gridData.worldSizeX / 2) / gridData.worldSizeX;
        float percentY = (worldPos.y + gridData.worldSizeY / 2) / gridData.worldSizeY;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridData.columns - 1) * percentX);
        int y = Mathf.RoundToInt((gridData.rows - 1) * percentY);

        return new Vector2Int(x, y);
    }

    public bool IsNodeEmpty(Vector2Int nodeIndex)
    {
        return _board[nodeIndex.x, nodeIndex.y] == null; //return true if empty
    }

    public Vector3 FromGridToWorldPosition(Vector2Int gridPosition) //Might not need this function
    {
        float x = this.transform.position.x - (_gridData.worldSizeX / 2f) + (gridPosition.x * _gridData.cellSize) + (_gridData.cellSize / 2f);
        float y = this.transform.position.y - (_gridData.worldSizeY / 2f) + (gridPosition.y * _gridData.cellSize) + (_gridData.cellSize / 2f);

        return new Vector3(x, y, this.transform.position.z);
    }

    public bool IsBlockOutOfBounds(Vector3 pos)
    {
        if (pos.x <= (transform.position.x - (_gridData.worldSizeX / 2f)))
            return true;
        else if (pos.x > (transform.position.x + (_gridData.worldSizeX / 2f)))
            return true;
        else if (pos.y <= (transform.position.y - (_gridData.worldSizeY / 2f)))
            return true;
        else if (pos.y > (transform.position.y + (_gridData.worldSizeY / 2f)))
            return true;

        return false;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if(_gridData != null)
        {
            Gizmos.color = Color.red;
            
            for(int i = 0; i < _gridData.columns; i++)
            {
                for(int j = 0; j < _gridData.rows; j++)
                {
                    float x = this.transform.position.x - (_gridData.worldSizeX / 2f) + (i * _gridData.cellSize) + (_gridData.cellSize / 2f);
                    float y = this.transform.position.y - (_gridData.worldSizeY / 2f) + (j * _gridData.cellSize) + (_gridData.cellSize / 2f);

                    Gizmos.DrawWireCube(new Vector3(x, y, transform.position.z), Vector3.one * _gridData.cellSize);
                }
            }

            Gizmos.color = Color.yellow;
            if(_pieceSpawnGridPosition != null) //Show where the piece is going to spawn by default
            {
                Gizmos.DrawCube(FromGridToWorldPosition(_pieceSpawnGridPosition), Vector3.one * _gridData.cellSize);
            }
        }
    }
}

public class Playfield
{
    private static PlayfieldManager _playfield;
    public static PlayfieldGrid gridData { get => _playfield.gridData; }
    
    public static void SetupGameboard(PlayfieldManager playfield)
    {
        _playfield = playfield;
    }

    public static Vector2Int FromWorldToGridPosition(Vector2 worldPos)
    {
        return _playfield.FromWorldToGridPosition(worldPos);
    }

    public static Vector3 FromGridToWorldPosition(Vector2Int gridPosition) //Might not need this function
    {
        return _playfield.FromGridToWorldPosition(gridPosition);
    }

    public static Vector3 GetPieceSpawnWorldPosition(bool isPivotOffsetted)
    {
        return _playfield.GetPieceSpawnWorldPosition(isPivotOffsetted);
    }

    public static bool IsPieceOutOfBounds(Vector2 pos)
    {
        return _playfield.IsBlockOutOfBounds(pos);
    }

    public static bool IsNodeEmpty(Vector2Int index)
    {
        return _playfield.IsNodeEmpty(index);
    }
}
