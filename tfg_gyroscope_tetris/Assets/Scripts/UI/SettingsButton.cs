using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _inactiveSprite;

    [SerializeField] private SETTINGS_OPTIONS _optionRef;

    [SerializeField] private Image _buttonImage;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private bool _state;

    private void Start()
    {
        switch (_optionRef)
        {
            case SETTINGS_OPTIONS.Vibration:
                SetState(Vibrator.enabled);
                break;
            case SETTINGS_OPTIONS.Audio:
                SetState(true);
                break;
            default:
                break;
        }
    }

    public void ToggleState()
    {
        ChangeSettingOption(_optionRef, !_state);
        SetState(!_state); 
    }

    private void SetState(bool state)
    {
        if (state)
            ActiveState();
        else
            InactiveState();


        _state = state;
    }

    private void ActiveState()
    {
        _buttonImage.sprite = _activeSprite;
        _buttonText.text = _optionRef.ToString() + ": ON"; 
    }

    private void InactiveState()
    {
        _buttonImage.sprite = _inactiveSprite; 
        _buttonText.text = _optionRef.ToString() + ": OFF";
    }

    private void ChangeSettingOption(SETTINGS_OPTIONS option, bool state)
    {
        switch (option)
        {
            case SETTINGS_OPTIONS.Audio: //Change audio settings
                break;
            case SETTINGS_OPTIONS.Vibration:
                Vibrator.enabled = state;
                break;
            default:
                break;
        }
    }
}

public enum SETTINGS_OPTIONS
{
    Audio, Vibration
}
