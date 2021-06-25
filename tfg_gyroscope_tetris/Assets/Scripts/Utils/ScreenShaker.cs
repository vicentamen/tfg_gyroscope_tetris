using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField, Min(0)] private float _duration = 0.5f;
    [SerializeField, Min(0)] private int _strength = 10;
    private Tween _animation;

    // Start is called before the first frame update
    void Start()
    {
        ScreenShake.screenShakeEvent.AddListener(DoShake);
    }

    private void OnEnable()
    {
        ScreenShake.screenShakeEvent.AddListener(DoShake);
    }

    private void OnDisable()
    {
        ScreenShake.screenShakeEvent.RemoveListener(DoShake);
    }

    private void DoShake(Vector3 direction)
    {
        direction = transform.rotation * direction;
        if(_animation == null || !_animation.IsPlaying())
            _animation = transform.DOPunchPosition(direction * _strength, _duration, 1, 0);
    }
}
