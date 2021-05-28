using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePanel : MonoBehaviour
{
    public Canvas canvas;
    private RectTransform _panelSafeArea;

    private Rect _currentSafeArea;
    private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait; //Our game is locked on portrait mode

    // Start is called before the first frame update
    void Start()
    {
        _panelSafeArea = GetComponent<RectTransform>();

        //store current values
        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;

        ApplySafeArea(); //If we need to Update the resolution or rotation in runtime 
    }

    private void ApplySafeArea()
    {
        if (_panelSafeArea == null)
            return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;

        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        _panelSafeArea.anchorMin = anchorMin;
        _panelSafeArea.anchorMax = anchorMax;

        _currentOrientation = Screen.orientation;
        _currentSafeArea = safeArea;
    }
}
