using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateWithScreen : MonoBehaviour
{
    [SerializeField] private ConstrainedPositions[] positions;
    // Start is called before the first frame update
    void Start()
    {
        Rotator.onRotateEvent.AddListener(OnRotate);
    }

    private void OnDisable()
    {
        Rotator.onRotateEvent.RemoveListener(OnRotate);
    }

    private void OnRotate(SCREEN_ORIENTATION orientation)
    {
        foreach (ConstrainedPositions pos in positions)
            if (orientation == pos.orientation)
                transform.position = pos.transform.position;
    }
}

[System.Serializable]
public class ConstrainedPositions
{
    public Transform transform;
    public SCREEN_ORIENTATION orientation;
}
