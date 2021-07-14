using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotationManager : MonoBehaviour
{

    public SCREEN_ORIENTATION orientation { get => _currentOrientation; }
    private SCREEN_ORIENTATION _currentOrientation;

    [SerializeField] private InputSystem _inputSystem;
    //[SerializeField] private float _rotationSpeed = 60f;
    [SerializeField] private float _safeArea = 20;
    private float _rotation = 0;

    //Rotation events
    [HideInInspector] public UnityEvent<SCREEN_ORIENTATION> onRotation = new UnityEvent<SCREEN_ORIENTATION>();

    public void Init()
    {
        Rotator.SetUpManager(this);

        //Setup orientations
        _currentOrientation = SCREEN_ORIENTATION.PORTRAIT;


        _rotation = 0f;
    }


    private void Update()
    {
        //_rotation += _inputSystem.GetRotation() * _rotationSpeed * Time.deltaTime;
        //_rotation = Input.gyro.attitude.eulerAngles.z - 90f;
        _rotation += _inputSystem.GetRotation() * Mathf.Rad2Deg * Time.deltaTime;
        //Keep rotation within 0 and 359
        if (_rotation >= 360f)
            _rotation -= 360f;
        else if (_rotation < 0f)
            _rotation += 360f; //The rotation will be negative here, that is why we are adding instead of substracting

        if (CheckRotation(_rotation))
        {
            SCREEN_ORIENTATION orientation = GetOrientationFromRotation(_rotation);
            Rotate(orientation);
        }
    }

    public void ResetRotation()
    {

    }

    private void Rotate(SCREEN_ORIENTATION newOrientation)
    {
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
        if ((rotation > 90f - 45f) &&
            (rotation <= 90f + 45f))
            return SCREEN_ORIENTATION.INV_LANDSCAPE;
        else if ((rotation > 180f - 45f) &&
            (rotation <= 180f + 45f))
            return SCREEN_ORIENTATION.INV_PORTRAIT;
        else if ((rotation > 270f - 45f) &&
            (rotation <= 270f + 45f))
            return SCREEN_ORIENTATION.LANDSCAPE;
        else if ((rotation <= 0f + 45f) ||
            (rotation > 360 - 45f))
            return SCREEN_ORIENTATION.PORTRAIT;

        return SCREEN_ORIENTATION.NONE;
    }

    private bool CheckRotation(float rotation)
    {
        if(_currentOrientation == SCREEN_ORIENTATION.LANDSCAPE)
        {
            if (_rotation < 270f - _safeArea || _rotation > 270f + _safeArea)
                return true;
        }
        else if(_currentOrientation == SCREEN_ORIENTATION.INV_PORTRAIT)
        {
            if (_rotation < 180f - _safeArea || _rotation > 180f + _safeArea)
                return true;
        }
        else if(_currentOrientation == SCREEN_ORIENTATION.INV_LANDSCAPE)
        {
            if (_rotation < 90f - _safeArea || _rotation > 90f + _safeArea)
                return true;
        }
        else if (_currentOrientation == SCREEN_ORIENTATION.PORTRAIT)
        {
            if (_rotation > 0f + _safeArea || _rotation < 360f - _safeArea)
                return true;
        }

        return false; //It does not have to rotate
    }
}

public enum SCREEN_ORIENTATION 
{ 
    PORTRAIT, LANDSCAPE, INV_PORTRAIT, INV_LANDSCAPE, NONE
}

public static class Rotator
{
    private static RotationManager _manager;
    public static UnityEvent<SCREEN_ORIENTATION> onRotateEvent { get => _manager.onRotation; set => _manager.onRotation = value; }

    public static SCREEN_ORIENTATION orientation { get => _manager.orientation; }

    public static void SetUpManager(RotationManager manager)
    {
        _manager = manager;
    }

    public static float GetRotationFromOrientation(SCREEN_ORIENTATION orientation)
    {
        return _manager.GetRotationFromOrientation(orientation);
    }

    public static void ResetRotation()
    {
        _manager.ResetRotation();
    }
}