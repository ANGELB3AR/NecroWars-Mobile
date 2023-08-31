using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHoard : MonoBehaviour
{
    [SerializeField] RectTransform button;
    [SerializeField] Transform hoard;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (hoard == null) { return; }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(hoard.position);

        button.position = screenPosition;
    }
}
