using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Transform playerHoardMovementTransform;

    public Hoard playerHoard;


    private void Start()
    {
        mainCamera = Camera.main;
        playerHoardMovementTransform = playerHoard.hoardMovementTransform;
    }

    private void Update()
    {
        MovePlayerHoard();
    }

    private void MovePlayerHoard()
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed) { return; }

        Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        touchPosition.z = mainCamera.transform.position.y;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.y = 0f;

        playerHoardMovementTransform.position = worldPosition;
    }
}
