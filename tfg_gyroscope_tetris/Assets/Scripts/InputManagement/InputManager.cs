using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private GameObject _testObject;
    [SerializeField] private float _moveSpeed = 9f;


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float h = _inputSystem.GetHorizontal();
        float v = _inputSystem.GetVertical();

        _testObject.transform.position = new Vector3(h, v) * _moveSpeed;
    }
}
