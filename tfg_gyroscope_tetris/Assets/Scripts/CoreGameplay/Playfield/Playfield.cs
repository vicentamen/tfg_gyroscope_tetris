using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    [SerializeField, Tooltip("Playfield grid data")]
    private PlayfieldGrid _gridData = null;
    [SerializeField, Tooltip("Reference to the container for the blocks gameobject placed on the playfield")]
    private Transform _blocksContainer;

    public PlayfieldGrid gridData { get => _gridData; }

    //Gotta have to save the grid state somewhere and the blocks linked to the positions
    private SpriteRenderer[,] _board; //It probably needs to be a block matrix. =========== WILL NEED TO FIND A WAY TO SAVE THE OBSTACLES POSITION IN THE BOARD ==============

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlacePiece(PieceBase piece)
    {
        //get the piece blocks position and add them to the board, will have to use BLOCK objects or not, maybe I can simply have SpriteReder-s and animate them later with DoTween
        //Will probably need a pool of bloks if I go with this solution
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

    public Vector2 FromGridToWorldPosition(Vector2Int gridPosition) //Might not need this function
    {
        Vector2 worldPos = new Vector2();
        //Calculate worldPosition
        return worldPos;
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
        }
    }
}
