using System;
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

    public event Action OnPlayerHoardInitiated;


    private void Start()
    {
        mainCamera = Camera.main;
        playerHoardMovementTransform = playerHoard.hoardMovementTransform;
    }

    private void Update()
    {
        if (playerHoardMovementTransform == null)
        {
            playerHoardMovementTransform = playerHoard.hoardMovementTransform;
        }

        HandlePlayerInput();
        LerpHoardTransform();
    }

    public void SetPlayerHoard(Hoard hoard)
    {
        playerHoard = hoard;

        OnPlayerHoardInitiated?.Invoke();
    }

    private void HandlePlayerInput()
    {
        if (!Application.isFocused) { return; }
        if (!Touchscreen.current.primaryTouch.press.isPressed) { return; }

        Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        touchPosition.z = mainCamera.transform.position.y;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out hit))
        {
            Creature creature = hit.collider.GetComponent<Creature>();
            
            if (creature != null)
            {
                HandleTapOnCreature(creature);
            }
            else
            {
                MovePlayerHoard(touchPosition);
            }
        }
    }

    private void HandleTapOnCreature(Creature creature)
    {
        if (!creature.GetDesignatedHoard().isPlayer) { return; }

        if (creature is IBonusAttack bonusAttackCreature)
        {
            bonusAttackCreature.BonusAttack();
        }
    }

    private void MovePlayerHoard(Vector3 touchPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.y = 0f;

        targetPosition = worldPosition;
    }

    private void LerpHoardTransform()
    {
        if (playerHoardMovementTransform.position == targetPosition) { return; }

        playerHoardMovementTransform.position = Vector3.Lerp(playerHoardMovementTransform.position, targetPosition, movementSpeed * Time.deltaTime);
    }
}
