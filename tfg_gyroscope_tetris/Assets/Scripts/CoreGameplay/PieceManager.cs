using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Manages the pieces that are given to the player
/// </summary>
public class PieceManager : MonoBehaviour
{
    private PieceSet _pieceSet;
    //Piece management
    private Queue<PieceBase> _piecesQueue; //pieces that the player is going to play
    private Queue<PieceBase> _backupPiecesQueue; //Secondary queue

    private List<PieceBase> _instancedPieces;
    [SerializeField] private Transform _piecesContainer;

    //UI pieces management
    [SerializeField] private Image _nextPieceImage;
    [SerializeField] private Image _secondNextPieceImage;
    [SerializeField] private Image _thirdNextPieceImage;

    private PieceBase _savedPiece; //Piece that the player has saved to use later

    public void Initialize(PieceSet gamePieces, PlayfieldGrid gridData)
    {
        _pieceSet = gamePieces;
        //Initilize game pieces
        InitPieces(gridData);
        //Build the main and back up pieces queue
        BuildPiecesQueue(out _piecesQueue);
        BuildPiecesQueue(out _backupPiecesQueue);

        InitPiecesUI();
    }

    private void InitPieces(PlayfieldGrid gridData)
    {
        _instancedPieces = new List<PieceBase>();
        //Instiate pieces and initialize them
        foreach(GameObject go in _pieceSet.pieces)
        {
            PieceBase piece = GameObject.Instantiate(go, _piecesContainer).GetComponent<PieceBase>(); //Instatiate piece
            piece.InitPiece(gridData); //InitializePiece

            piece.gameObject.SetActive(false); //Hide instatiated piece

            _instancedPieces.Add(piece); //Add piece to the list
        }
    }

    private void InitPiecesUI()
    {
        UpdateNextPiecesIU();
    }

    /// <summary>
    /// Returns the next piece in the queue
    /// </summary>
    /// <returns>Returns a PieceBase or null if there has not been any</returns>
    public PieceBase GetNextPiece()
    {
        if (_piecesQueue == null)
        {
            Debug.LogError("There are no pieces in the queue");
            return null;
        }

        PieceBase nextPiece = _piecesQueue.Dequeue();
        _piecesQueue.Enqueue(_backupPiecesQueue.Dequeue());

        if (_backupPiecesQueue.Count <= 0) //If we have emptied the backup queue, fill it again
            BuildPiecesQueue(out _backupPiecesQueue);

        UpdateNextPiecesIU();

        return nextPiece;
    }

    private void BuildPiecesQueue(out Queue<PieceBase> queue)
    {
        queue = new Queue<PieceBase>();

        //Copy the instantiated pieces List
        PieceBase[] setCopy = new PieceBase[_instancedPieces.Count];
        _instancedPieces.CopyTo(setCopy);

        //Shuffle copied List
        ShufflePieceList(setCopy);
        //add shuffled pieces to the queue, they could be added directly in the ShufflePieceFunction
        foreach (PieceBase p in setCopy)
            queue.Enqueue(p);
    }

    //I could simply extend the list class and add the function to it, but too don't have time rn
    private void ShufflePieceList(PieceBase[] list)
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            PieceBase aux = list[k];
            list[k] = list[n];
            list[n] = aux;
        }
    }

    public void UpdateNextPiecesIU()
    {
        PieceBase[] nextPieces = _piecesQueue.ToArray();
        _nextPieceImage.sprite = nextPieces[0].menuPreviewSprite;
        _secondNextPieceImage.sprite = nextPieces[1].menuPreviewSprite;
        _thirdNextPieceImage.sprite = nextPieces[2].menuPreviewSprite;
    }
}
