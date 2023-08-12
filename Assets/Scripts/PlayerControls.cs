using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private void Update()
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed) { return; }

        Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
    }
}
