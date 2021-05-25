using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Keyboard Input", menuName = "Input Systems/Keyboard Input")]
public class KeyboardInputSystem : InputSystem
{
    public override float GetHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override float GetVertical()
    {
        return Input.GetAxisRaw("Vertical");
    }
}
