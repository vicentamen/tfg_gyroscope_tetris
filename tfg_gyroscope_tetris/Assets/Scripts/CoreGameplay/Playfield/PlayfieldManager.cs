using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

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

    public float highestLine { get => _highestLine; }
    private int _highestLine; 

    public void Initialize()
    {
        _board = new Block[_gridData.columns, _gridData.rows];
        _pieceSpawnGridPosition = new Vector2Int(Mathf.RoundToInt(_gridData.columns / 2), (_gridData.rows < 26) ? (_gridData.rows - 2) : 24);

        Playfield.SetupGameboard(this);

        _highestLine = 0;
    }


    public Vector3 GetPieceSpawnWorldPosition(bool isPivotOffsetted)
    {
        //The result should change depending on wheter the pivot is offsetted or not
        float offset = (isPivotOffsetted) ? _gridData.cellSize / 2f : 0f;
        return FromGridToWorldPosition(_pieceSpawnGridPosition) - Vector3.one * offset;
    }

    public PiecePlaceResult PlacePiece(PieceBase piece)
    {
        PiecePlaceResult result = new PiecePlaceResult();
        //get the piece blocks position and add them to the board, will have to use BLOCK objects or not, maybe I can simply have SpriteReder-s and animate them later with DoTween
        foreach(Transform pieceBlock in piece.pieceBlocks)
        {
            Block newBlock = _blocksPool.GetItem().GetComponent<Block>();
            newBlock.gameObject.SetActive(true);

            newBlock.PlaceBlock(pieceBlock.position, Vector3.one * _gridData.cellSize, piece);

            //Save the block in the board
            Vector2Int index = FromWorldToGridPosition(pieceBlock.position);
            _board[index.x, index.y] = newBlock;

            //Set highest line
            if (index.y > _highestLine)
                _highestLine = index.y;

            result.AddOccuppiedLine(index.y);
        }

        for(int i = 0; i < result.occupiedLines.Count; i++)
            if (CheckCompletedLine(result.occupiedLines[i]))
                result.AddCompletedLine(result.occupiedLines[i]);

        return result;
    }

    public bool CheckCompletedLine(int line)
    {
        bool lineFull = true;
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            if(_board[i, line] == null)
            {
                lineFull = false;
                break;
            }
        }

        return lineFull;
    }

    public void ClearLines(List<int> lines)
    {
        Sequence clearSeq = DOTween.Sequence();
        for (int i = 0; i < lines.Count; i++)
            clearSeq.Join(ClearLine(lines[i]));

        clearSeq.AppendCallback(() => MakeBlocksFall(lines));

        _highestLine = Mathf.Clamp(_highestLine - lines.Count, 0, _gridData.rows);
    }

    public Sequence ClearLine(int line)
    {
        Sequence clearSeq = DOTween.Sequence();
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            clearSeq.Join(_board[i, line].DestroyAnimation(_board[i, line].DestroyBlock));
            _board[i, line] = null;
        }

        return clearSeq;
    }

    public void ClearAllLines(UnityAction onComplete)
    {
        Sequence clearSeq = DOTween.Sequence();
        for(int i = 0; i < _board.GetLength(0); i++)
        {
            for(int j = 0; j < _board.GetLength(1); j++)
            {
                if(_board[i, j] != null)
                {
                    clearSeq.Join(_board[i, j].DestroyAnimation(_board[i, j].DestroyBlock).SetDelay(0.05f * j));
                }
            }
        }

        clearSeq.AppendCallback(() => onComplete?.Invoke());
    }

    private void MakeBlocksFall(List<int> lines)
    {
        lines.Sort();
        for(int i = 0; i < lines.Count; i++)
        {
            MakeBlocksFall(lines[i], i);
        }
    }

    private void MakeBlocksFall(int startLine, int index)
    {
        for(int i = 0; i < _board.GetLength(0); i++)
        {
            for(int j = (startLine - index) + 1; j < _board.GetLength(1); j++)
            {
                if(_board[i, j] != null)
                {
                    _board[i, j].transform.position = Playfield.FromGridToWorldPosition(i, j - 1);
                    _board[i, j - 1] = _board[i, j];
                    _board[i, j] = null; //delete reference to the previous block
                }
            }
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
        return FromGridToWorldPosition(gridPosition.x, gridPosition.y);
    }

    public Vector3 FromGridToWorldPosition(int i, int j)
    {
        float x = this.transform.position.x - (_gridData.worldSizeX / 2f) + (i * _gridData.cellSize) + (_gridData.cellSize / 2f);
        float y = this.transform.position.y - (_gridData.worldSizeY / 2f) + (j * _gridData.cellSize) + (_gridData.cellSize / 2f);

        return new Vector3(x, y, this.transform.position.z);

    }

    public bool IsBlockOutOfBounds(Vector3 pos)
    {
        return IsBlockOutBottom(pos.y) || IsBlockOutTop(pos.y) || IsBlockOutRight(pos.x) || IsBlockOutLeft(pos.x);
    }

    public bool IsBlockOutLeft(float posX)
    {
        return posX <= (transform.position.x - (_gridData.worldSizeX / 2f));
    }

    public bool IsBlockOutRight(float posX)
    {
        return posX > (transform.position.x + (_gridData.worldSizeX / 2f));
    }

    public bool IsBlockOutBottom(float posY)
    {
        return posY <= (transform.position.y - (_gridData.worldSizeY / 2f));
    }

    public bool IsBlockOutTop(float posY)
    {
        return posY > (transform.position.y + (_gridData.worldSizeY / 2f));
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

public class PiecePlaceResult
{
    public List<int> occupiedLines { get => _occuppiedLines; }
    private List<int> _occuppiedLines;

    public List<int> completedLines { get => _completedLines; }
    private List<int> _completedLines;

    public int completedCount { get => _completedLines.Count; }

    public PiecePlaceResult()
    {
        _occuppiedLines = new List<int>();
        _completedLines = new List<int>();
    }

    public void AddOccuppiedLine(int line)
    {
        if (!_occuppiedLines.Contains(line)) //If the line is not yet on the list 
            _occuppiedLines.Add(line);
            
    }

    public void AddCompletedLine(int line)
    {
        if (!_completedLines.Contains(line)) //if the line is not already on the list
            _completedLines.Add(line);
    }
}
public class PlaceEvent : UnityEvent<PiecePlaceResult> { }

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

    public static Vector3 FromGridToWorldPosition(int i, int j)
    {
        return _playfield.FromGridToWorldPosition(i, j);
    }

    public static Vector3 GetPieceSpawnWorldPosition(bool isPivotOffsetted)
    {
        return _playfield.GetPieceSpawnWorldPosition(isPivotOffsetted);
    }

    public static bool IsBlockOutOfBounds(Vector2 pos)
    {
        return _playfield.IsBlockOutOfBounds(pos);
    }

    public static bool IsBlockOutRight(float posX)
    {
        return _playfield.IsBlockOutRight(posX);
    }

    public static bool IsBlockOutLeft(float posX)
    {
        return _playfield.IsBlockOutLeft(posX);
    }

    public static bool IsBlockOutTop(float posY)
    {
        return _playfield.IsBlockOutTop(posY);
    }

    public static bool IsClockOutBottom(float posY)
    {
        return _playfield.IsBlockOutBottom(posY);
    }

    public static bool IsNodeEmpty(Vector2Int index)
    {
        return _playfield.IsNodeEmpty(index);
    }

    public static bool IsBoardLinesFull()
    {
        return _playfield.highestLine > _playfield.gridData.columns;
    }

    public static void ClearBoard(UnityAction onComplete)
    {
        _playfield.ClearAllLines(onComplete);
    }
}
