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
        _state = true;
        ActiveState(); 
    }

    public void ToggleState()
    {
        ChangeSettingOption(_optionRef, !_state);

        if (_state)
            InactiveState();
        else
            ActiveState();


        _state = !_state;
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
