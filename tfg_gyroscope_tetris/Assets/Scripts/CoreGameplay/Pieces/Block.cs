using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlaceBlock(Vector3 position, Vector3 size, PieceBase piece)
    {
        SetBlockToPieceType(piece);
        transform.position = position;
        transform.localScale = size;

        gameObject.SetActive(true);
    }

    public void DestroyBlock()
    {
        //Should execute and animation for dissapearing using DoTween
        gameObject.SetActive(false);
    }

    private void SetBlockToPieceType(PieceBase piece)
    {
        _spriteRenderer.sprite = piece.blockSprite;//Set sprite to type
    }
}
