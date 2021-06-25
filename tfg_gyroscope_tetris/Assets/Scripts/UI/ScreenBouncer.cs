using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenBouncer : MonoBehaviour
{
    [SerializeField, Min(0)] private float _duration = 0.5f;
    [SerializeField] private int _strenght = 10;

    private Tween _animation;

    // Start is called before the first frame update
    void Start()
    {
        ScreenShake.screenBounceEvent.AddListener(DoBounce);
    }

    private void OnEnable()
    {
        ScreenShake.screenBounceEvent.AddListener(DoBounce);
    }

    private void OnDisable()
    {
        ScreenShake.screenBounceEvent.RemoveListener(DoBounce);
    }

    private void DoBounce(Vector3 direction)
    {
        direction = transform.rotation * direction; //Make the direction 
        if(_animation == null || !_animation.IsPlaying())
            _animation = transform.DOPunchPosition(direction * _strenght, _duration, 1);
    }
}
