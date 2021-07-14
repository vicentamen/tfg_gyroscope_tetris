using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _safePanelTransform;
    [SerializeField] private RectTransform _menuTransform;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    private CanvasGroup _canvasGroup;

    private UnityAction _onGameResumed;
    private SCREEN_ORIENTATION _startOrientation;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _startOrientation = SCREEN_ORIENTATION.PORTRAIT;

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the pause menu
    /// </summary>
    /// <param name="onGameResume">What to do when the game is resumed</param>
    /// <param name="onGameExit">What to do if the player exits the game</param>
    public void ShowMenu(SCREEN_ORIENTATION orientation, UnityAction onGameResume)
    {
        //Save unity actions
        _onGameResumed = onGameResume;

        _startOrientation = orientation;
        AdaptToOrientation(_startOrientation);

        Rotator.onRotateEvent.AddListener(AdaptToOrientation);

        _pauseButton.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        ShowAnimation();
    }

    private void ShowAnimation()
    {
        //Do the show animation
        //Enable buttons after when the animation is over

    }

    /// <summary>
    /// Hides the menu using an animation
    /// </summary>
    /// <param name="onComplete">What to do when the hode animation is complete</param>
    private void HideMenu(UnityAction onComplete)
    {
        Rotator.onRotateEvent.RemoveListener(AdaptToOrientation);

        onComplete?.Invoke();

        _pauseButton.gameObject.SetActive(true); 
        this.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        HideMenu(() => GameManager.LoadScene(GAME_SCENES.StartScene));
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game"); 
        HideMenu(_onGameResumed);
    }

    private void AdaptToOrientation(SCREEN_ORIENTATION orientation)
    {
        //Rotate to the orientation rotation
        _menuTransform.rotation = Quaternion.Euler(0f, 0f, Rotator.GetRotationFromOrientation(orientation));

        //Adapt menu size to orientation
        SetMenuSize(orientation);

        //Enable/Disable resume button depending on the orientation
        if (orientation != _startOrientation)
            _resumeButton.interactable = false;
        else
            _resumeButton.interactable = true;

    }

    private void SetMenuSize(SCREEN_ORIENTATION orientation)
    {
        _menuTransform.position = _safePanelTransform.position;
        if(orientation == SCREEN_ORIENTATION.LANDSCAPE || orientation == SCREEN_ORIENTATION.INV_LANDSCAPE)
        {
            Vector2 size = new Vector2(_safePanelTransform.rect.height, _safePanelTransform.rect.width);

            _menuTransform.sizeDelta = size;
        }
        else
        {
            _menuTransform.sizeDelta = _safePanelTransform.rect.size;
        }
    }
}
