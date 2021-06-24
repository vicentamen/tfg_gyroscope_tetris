using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstrainedRotateWithScreen : MonoBehaviour
{
    [SerializeField] private RotationConstrains[] constrains;
    private void Start()
    {
        Rotator.onRotateEvent.AddListener(OnRotation);
    }

    private void OnDisable()
    {
        Rotator.onRotateEvent.RemoveListener(OnRotation);
    }

    private void OnRotation(SCREEN_ORIENTATION orientation)
    {
        foreach(RotationConstrains c in constrains)
        {
            if(orientation == c.orientation)
            {
                transform.DORotate(new Vector3(0f, 0f, c.rotation), 0.5f);
            }
        }
    }
}

[System.Serializable]
public class RotationConstrains
{
    public float rotation;
    public SCREEN_ORIENTATION orientation;
}
