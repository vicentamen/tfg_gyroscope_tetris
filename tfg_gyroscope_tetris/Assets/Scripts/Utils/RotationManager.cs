using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotationManager : MonoBehaviour
{

    public SCREEN_ORIENTATION orientation { get => _currentOrientation; }
    private SCREEN_ORIENTATION _currentOrientation;

    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _safeArea = 20;
    private float _rotation = 0;

    //Rotation events
    [HideInInspector] public UnityEvent<SCREEN_ORIENTATION> onRotation;

    private void Start()
    {
        Rotator.SetUpManager(this);

        //Init unity events
        onRotation = new UnityEvent<SCREEN_ORIENTATION>();

        //Setup orientations
        _currentOrientation = SCREEN_ORIENTATION.PORTRAIT;

        _rotation = 0;
    }

    private void Update()
    {
        _rotation += _inputSystem.GetRotation() * _rotationSpeed * Time.deltaTime;
        //_rotation = Input.gyro.attitude.eulerAngles.z;
        //Keep rotation within 0 and 359
        if (_rotation >= 360f)
            _rotation -= 360f;
        else if (_rotation < 0f)
            _rotation = 360 + _rotation; //The rotation will be negative here, that is why we are adding instead of substracting

        SCREEN_ORIENTATION orientation = GetOrientationFromRotation(_rotation);
        Rotate(orientation);
    }

    private void Rotate(SCREEN_ORIENTATION newOrientation)
    {
        if(newOrientation != SCREEN_ORIENTATION.NONE && newOrientation != _currentOrientation)
            _currentOrientation = newOrientation;

        onRotation?.Invoke(newOrientation);
    }

    public float GetRotationFromOrientation(SCREEN_ORIENTATION orientation)
    {
        switch (orientation)
        {
            case SCREEN_ORIENTATION.LANDSCAPE: return 270f;
            case SCREEN_ORIENTATION.INV_PORTRAIT: return 180f;
            case SCREEN_ORIENTATION.INV_LANDSCAPE: return 90f;
            case SCREEN_ORIENTATION.PORTRAIT:
            default: return 0f;
        }
    }   

    public SCREEN_ORIENTATION GetOrientationFromRotation(float rotation)
    {
        if ((rotation <= 0f + _safeArea) ||
            (rotation >=  360f - _safeArea))
            return SCREEN_ORIENTATION.PORTRAIT;
        else if ((rotation >= 90f - _safeArea) &&
            (rotation <= 90f + _safeArea))
            return SCREEN_ORIENTATION.INV_LANDSCAPE;
        else if ((rotation >= 180f - _safeArea) &&
            (rotation <= 180f + _safeArea))
            return SCREEN_ORIENTATION.INV_PORTRAIT;
        else if ((rotation <= 270f + _safeArea) &&
            (rotation >= 270f - _safeArea))
            return SCREEN_ORIENTATION.LANDSCAPE;

        return SCREEN_ORIENTATION.NONE;
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.TextArea(new Rect(0f, 0f, 200f, 200f), _rotation.ToString());
    }
}

public enum SCREEN_ORIENTATION 
{ 
    PORTRAIT, LANDSCAPE, INV_PORTRAIT, INV_LANDSCAPE, NONE
}

public static class Rotator
{
    private static RotationManager _manager;

    public static SCREEN_ORIENTATION orientation { get => _manager.orientation; }

    public static void SetUpManager(RotationManager manager)
    {
        _manager = manager;
    }

    public static float GetRotationFromOrientation(SCREEN_ORIENTATION orientation)
    {
        return _manager.GetRotationFromOrientation(orientation);
    }
}