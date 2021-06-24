using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateWithScreen : MonoBehaviour
{
    void Start()
    {
        Rotator.onRotateEvent.AddListener(OnRotation);
    }

    private void OnDisable()
    {
        Rotator.onRotateEvent.RemoveListener(OnRotation);
    }

    private void OnRotation(SCREEN_ORIENTATION orientation)
    {
        float rotation = Rotator.GetRotationFromOrientation(orientation);
        transform.DORotate(new Vector3(0f, 0f, rotation), 0.5f);
    }
}
