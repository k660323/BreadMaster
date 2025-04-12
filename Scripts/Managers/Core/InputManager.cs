using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager
{
    public Vector2 inputDir;

    public void Init()
    {
        inputDir = Vector2.zero;
    }
}
