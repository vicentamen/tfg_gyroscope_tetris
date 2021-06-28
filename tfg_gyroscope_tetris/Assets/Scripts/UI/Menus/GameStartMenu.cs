using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [SerializeField] private Transform[] _buttons;
    [SerializeField] private Image _frontTint;

    Sequence _anim; 

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in _buttons)
            t.localScale = Vector3.zero;

        _frontTint.color = new Color(_frontTint.color.r, _frontTint.color.g, _frontTint.color.b, 1f);

        ShowAnimation();
    }
    
    public void OnStart()
    {
        HideAnimation(() => GameManager.ChangeScene(GAME_SCENES.CoreLoopScene));
    }

    private void OnSettings()
    {

    }

    private void ShowAnimation()
    {
        _anim = DOTween.Sequence();
        _anim.Join(_frontTint.DOFade(0f, .8f));
        for (int i = 0; i < _buttons.Length; i++)
            _anim.Join(_buttons[i].DOScale(1f, 1f).SetEase(Ease.OutBack).SetDelay(0.5f * i));

        _anim.Play();
    }

    private void HideAnimation(UnityAction onComplete = null)
    {
        _anim = DOTween.Sequence();
        for (int i = 0; i < _buttons.Length; i++)
            _anim.Join(_buttons[i].DOScale(0f, .5f).SetEase(Ease.InBack).SetDelay(0.1f * i));
        _anim.Join(_frontTint.DOFade(1f, .4f).SetDelay(.1f));
        _anim.AppendCallback(() => onComplete?.Invoke());
    }
}
