using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class FollowCamInitializer : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera followCam;

    private void Update()
    {
        if (followCam.m_Follow != null) { return; }

        InitializeFollowCam();
    }

    private void InitializeFollowCam()
    {
        Transform cameraTarget = FindObjectOfType<PlayerController>().playerHoard.hoardMovementTransform;

        followCam.m_Follow = cameraTarget;
    }
}
