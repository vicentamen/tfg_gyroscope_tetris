using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    [SerializeField, Tooltip("Playfield grid data")]
    private PlayfieldGrid _gridData = null;
    [SerializeField, Tooltip("Reference to the container for the blocks gameobject placed on the playfield")]
    private Transform _blocksContainer;
    [SerializeField]
    private Transform _backgroundImage;
    [Header("BLOCK PROPERTIES")]
    [SerializeField]
    private Pool _blocksPool;

    public PlayfieldGrid gridData { get => _gridData; }

    //Gotta have to save the grid state somewhere and the blocks linked to the positions
    private SpriteRenderer[,] _board; //It probably needs to be a block matrix. =========== WILL NEED TO FIND A WAY TO SAVE THE OBSTACLES POSITION IN THE BOARD ==============\\

    //Piece placement management
    private Vector2Int _pieceSpawnGridPosition;

    public void Initialize()
    {
        UpdateBackgroundImage();

        _pieceSpawnGridPosition = new Vector2Int(Mathf.RoundToInt(_gridData.columns / 2), (_gridData.rows < 26)? (_gridData.rows - 2) : 24);
    }

    private void UpdateBackgroundImage()
    {
        SpriteRenderer image;
        if(_backgroundImage.TryGetComponent(out image))
        {
            image.size = new Vector2(_gridData.worldSizeX + (_gridData.cellSize/2f), _gridData.worldSizeY); //Update Image Size
        }

        //Update image position
        //_backgroundImage.transform.position = this.transform.position + (Vector3.down * (_gridData.cellSize/2f));
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
            //Now I should save the block somewhere so I can delete it later
        }
        //Will probably need a pool of bloks if I go with this solution
        //Spawn blocks
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

    public Vector3 FromGridToWorldPosition(Vector2Int gridPosition) //Might not need this function
    {
        float x = this.transform.position.x - (_gridData.worldSizeX / 2f) + (gridPosition.x * _gridData.cellSize) + (_gridData.cellSize / 2f);
        float y = this.transform.position.y - (_gridData.worldSizeY / 2f) + (gridPosition.y * _gridData.cellSize) + (_gridData.cellSize / 2f);

        return new Vector3(x, y, this.transform.position.z);
    }

    public bool IsOutOfBounds(Vector3 pos)
    {
        if (pos.x < this.transform.position.x - (_gridData.worldSizeX / 2f) + (_gridData.cellSize / 2f))
            return true;
        else if (pos.x > this.transform.position.x + (_gridData.worldSizeX / 2f) + (_gridData.cellSize / 2f))
            return true;
        else if (pos.y < this.transform.position.y - (_gridData.worldSizeY / 2f) + (_gridData.cellSize / 2f))
            return true;
        else if (pos.y > this.transform.position.y + (_gridData.worldSizeY / 2f) + (_gridData.cellSize / 2f))
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
