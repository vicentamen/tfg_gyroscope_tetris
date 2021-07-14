using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private RectTransform _playAgainButton;
    [SerializeField] private RectTransform _exitButton;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        HideMenu();
    }

    public void HideMenu()
    {
        _canvasGroup.alpha = 0f;
        this.gameObject.SetActive(false);
    }

    public void ShowMenu(int finalScore)
    {
        _playAgainButton.localScale = Vector3.zero;
        _exitButton.localScale = Vector3.zero;

        _canvasGroup.alpha = 0f;

        this.gameObject.SetActive(true);

        ShowAnimation(finalScore, ShowButtons);
    }

    private void ShowAnimation(int score, UnityAction onComplete)
    {
        _canvasGroup.DOFade(1f, 0.5f);
        _scoreText.DOCounter(0, score, 2f).SetEase(Ease.OutCubic).OnComplete(onComplete.Invoke);
    }

    private void ShowButtons()
    {
        _playAgainButton.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
        _exitButton.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
    }

    public void OnPlayAgain()
    {
        GameManager.ResetScene(); //Reload the current scene
    }

    public void OnExit()
    {
        GameManager.LoadScene(GAME_SCENES.StartScene); //Go to the main menu
    }
}
