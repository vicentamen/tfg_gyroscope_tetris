using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Touch and Gyro Input System", menuName = "Input Systems/Touch and Gyro Input")]
public class TouchAndGyroInputSystem : InputSystem
{
    [SerializeField, Range(0, 1)] 
    private float _horizontalSens = 0.2f;
    [SerializeField, Range(0, 1)]
    private float _verticalSens = 0.5f;
    public override float GetHorizontal()
    {
        if (!Input.gyro.enabled)
            Input.gyro.enabled = true;

        Debug.Log(Screen.orientation);

        float inputX = Input.acceleration.x;
        if (inputX < -_horizontalSens)
            inputX = -1;
        else if (inputX > _horizontalSens)
            inputX = 1;
        else
            inputX = 0;

        return inputX;
    }

    public override float GetRotation()
    {
        return Input.acceleration.z;
    }

    public override float GetVertical()
    {
        if (!Input.gyro.enabled)
            Input.gyro.enabled = true;

        float inputY = Input.acceleration.y;
        if (inputY < -_verticalSens)
            inputY = -1;
        else if (inputY > _verticalSens)
            inputY = 1;
        else
            inputY = 0; 

        return inputY;
    }
}
