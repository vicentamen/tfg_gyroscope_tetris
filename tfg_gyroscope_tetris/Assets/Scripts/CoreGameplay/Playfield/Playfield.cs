using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
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
        _pieceSpawnGridPosition = new Vector2Int(Mathf.RoundToInt(_gridData.columns / 2), (_gridData.rows < 26)? (_gridData.rows - 2) : 24);
    }

    public Vector3 GetPieceSpawnWorldPosition(bool isPivotOffsetted)
    {
        //The result should change depending on wheter the pivot is offsetted or not
        return FromGridToWorldPosition(_pieceSpawnGridPosition);
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

    public bool IsPieceOutOfBounds(Rect pieceRect)
    {
        if (pieceRect.xMin <= (transform.position.x - (_gridData.worldSizeX / 2f) + (_gridData.cellSize / 2f)))
            return true;
        else if ((pieceRect.xMax - 1) > (transform.position.x + (_gridData.worldSizeX / 2f) + (_gridData.cellSize / 2f)))
            return true;
        else if (pieceRect.yMin <= (transform.position.y - (_gridData.worldSizeY / 2f) + (_gridData.cellSize / 2f)))
            return true;
        else if (pieceRect.yMax > (transform.position.y + (_gridData.worldSizeY / 2f) + (_gridData.cellSize / 2f)))
            return true;

        return false; //if none of the above is true then the piece is not out of bonds
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
