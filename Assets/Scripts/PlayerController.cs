using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Hoard playerHoard;

    private Camera mainCamera;
    private Transform playerHoardMovementTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        playerHoardMovementTransform = playerHoard.GetComponent<Transform>();
    }

    private void Update()
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed) { return; }

        Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        playerHoardMovementTransform.Translate(worldPosition);
    }
}
