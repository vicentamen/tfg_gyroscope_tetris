using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _highlightSpriteRenderer;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.color = Color.white;
        _highlightSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);//Make the highlight transparent
    }

    public void PlaceBlock(Vector3 position, Vector3 size, PieceBase piece)
    {
        SetBlockToPieceType(piece);
        transform.position = position;
        transform.localScale = size;

        PlaceAnimation();
    }

    public void DestroyBlock()
    {
        gameObject.SetActive(false);
    }

    private void SetBlockToPieceType(PieceBase piece)
    {
        if (piece == null)
            return;
        _spriteRenderer.sprite = piece.blockSprite;//Set sprite to type
    }

    private void PlaceAnimation()
    {
        Sequence placeAnim = DOTween.Sequence();
        int loops = 2;

        placeAnim.Append(_highlightSpriteRenderer.DOFade(0.75f, 0.25f / loops).SetEase(Ease.OutCubic));
        placeAnim.Append(_highlightSpriteRenderer.DOFade(0f, 0.25f / loops).SetEase(Ease.InCubic));

        placeAnim.SetLoops(loops, LoopType.Restart);
    }

    public Sequence DestroyAnimation(UnityAction onComplete)
    {
        Sequence destroyAnim = DOTween.Sequence();
        destroyAnim.Append(_highlightSpriteRenderer.DOFade(1f, 0.25f).SetEase(Ease.OutCubic));
        destroyAnim.Append(transform.DOScale(0f, 0.25f).SetEase(Ease.OutCubic));
        destroyAnim.Join(_highlightSpriteRenderer.DOFade(0f, 0.25f).SetEase(Ease.OutCubic));
        destroyAnim.Join(_spriteRenderer.DOFade(0f, 0.25f).SetEase(Ease.OutCubic));

        destroyAnim.AppendCallback(() => onComplete?.Invoke());

        return destroyAnim;
    }
}
