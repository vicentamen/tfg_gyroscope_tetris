using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public abstract class InputSystem : ScriptableObject
{
    /// <summary>
    /// Filtered horizontal input value
    /// </summary>
    /// <returns>A float value that goes from -1 to 1</returns>
    public abstract float GetHorizontal();
    /// <summary>
    /// Filtered vertical input value
    /// </summary>
    /// <returns>A float value that goes form -1 to 1</returns>
    public abstract float GetVertical();

    /// <summary>
    /// Filtered rotation direction
    /// </summary>
    /// <returns>float value that goes from -1 to 1</returns>
    public abstract float GetRotation();
}
