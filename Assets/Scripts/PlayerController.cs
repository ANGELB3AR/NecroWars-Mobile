using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    private Camera mainCamera;
    private Transform playerHoardMovementTransform;
    private Vector3 targetPosition;

    public Hoard playerHoard;


    private void Start()
    {
        mainCamera = Camera.main;
        playerHoardMovementTransform = playerHoard.hoardMovementTransform;
    }

    private void Update()
    {
        MovePlayerHoard();

        LerpHoardTransform();
    }

    private void MovePlayerHoard()
    {
        if (!Application.isFocused) { return; }
        if (!Touchscreen.current.primaryTouch.press.isPressed) { return; }

        Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        touchPosition.z = mainCamera.transform.position.y;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.y = 0f;

        targetPosition = worldPosition;
    }

    private void LerpHoardTransform()
    {
        if (playerHoardMovementTransform.position != targetPosition)
        {
            playerHoardMovementTransform.position = Vector3.Lerp(playerHoardMovementTransform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }
}
