using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConstrainedOpacityWithScreen : MonoBehaviour
{
    [SerializeField] private OpacityConstrain[] _constrains;
    private CanvasGroup _canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Rotator.onRotateEvent.AddListener(OnRotation); 
    }

    private void OnDisable()
    {
        Rotator.onRotateEvent.RemoveListener(OnRotation); 
    }

    private void OnRotation(SCREEN_ORIENTATION orientation)
    {
        foreach (OpacityConstrain constrain in _constrains)
            if (orientation == constrain.orientation)
                _canvasGroup.DOFade(constrain.opacity, 0.25f); 

    }
}

[System.Serializable]
public class OpacityConstrain
{
    public SCREEN_ORIENTATION orientation;
    public float opacity;
}
