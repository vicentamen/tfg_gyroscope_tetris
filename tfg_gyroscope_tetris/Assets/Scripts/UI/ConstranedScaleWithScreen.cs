using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstranedScaleWithScreen : MonoBehaviour
{
    [SerializeField] private List<ScaleContrain> _constrains;

    // Start is called before the first frame update
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
        foreach (ScaleContrain constrain in _constrains)
            if (orientation == constrain.orientation)
                transform.DOScale(constrain.scale, 0.1f);
    }
}

[System.Serializable]
public class ScaleContrain
{
    public SCREEN_ORIENTATION orientation;
    public float scale;
}
