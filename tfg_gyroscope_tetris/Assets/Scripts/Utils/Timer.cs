using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; //Need the events to build the callback using a UnityAction (could use a delegate though)

/// <summary>
/// Executes a function after the time given has been passed
/// </summary>
public class Timer
{
    private float _time;
    private UnityAction _callback;
    private float _timer; //The time that has elapsed since the timer was created
    private bool _isTimerRunning = false;

    public Timer(float time, UnityAction callback, bool startImmediate = false)
    {
        _time = time;
        _callback = callback;
        _timer = 0f;

        if (startImmediate) StartTimer();
    }


    public void Update()
    {
        _timer += Time.deltaTime; //Increment time with the time past since the last Update
        if ((_timer >= _time) && _isTimerRunning) //If the time passed is equals or bigger excute the callback function passed
        {
            _callback?.Invoke(); //We add the question mark to make sure that the callback is not null, otherwise we would find an error trying to execute it
            _isTimerRunning = false;
        }
    }

    public void StartTimer()
    {
        _isTimerRunning = true;
    }

    public void PauseTimer()
    {
        _isTimerRunning = !_isTimerRunning; //change the state of the timer started to the opposite
    }
}
