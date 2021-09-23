using System;
using System.Collections;
using System.Collections.Generic;
using Managers.SO;
using UnityEngine;

/// <summary>
/// Class used when the game needs a different camera
/// </summary>
public class CameraZone : MonoBehaviour
{
    [Tooltip("Channel file")]
    [SerializeField] private CameraEventsChannelSO cameraEventsChannelSo;

    [Tooltip("Choose behaviour for this trigger")]
    [SerializeField] private CameraZoneMode cameraZoneMode = CameraZoneMode.Base;
    
    [Space] [Header("Fixed mode")]
    [Tooltip("Transform where tu put camera")]
    [SerializeField] private Transform fixedCameraPos;
    [Tooltip("If true the fixed camera looks at the player")]
    [SerializeField] private bool lookAtPlayer;
    
    [Space] [Header("Zoom mode")]
    [Tooltip("Distance propertie of the CM camera")]
    [SerializeField] private float newDistance = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (cameraZoneMode)
            {
                case CameraZoneMode.Fixed:
                    cameraEventsChannelSo.RaiseOnEnterFixedRoomCallback(fixedCameraPos.position, fixedCameraPos.rotation, lookAtPlayer);
                    break;
                case CameraZoneMode.Zoom:
                    cameraEventsChannelSo.RaiseOnEnterZoomRoomCallback(newDistance);
                    break;
                case CameraZoneMode.Base:
                    cameraEventsChannelSo.RaiseOnEnterBaseRoomCallback();
                    break;
            }
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (cameraZoneMode)
            {
                case CameraZoneMode.Fixed:
                    cameraEventsChannelSo.RaiseOnExitFixedRoomCallback();
                    break;
                case CameraZoneMode.Zoom:
                    cameraEventsChannelSo.RaiseOnExitZoomRoomCallback();
                    break;
            }
        }
    }*/
}

enum CameraZoneMode
{
    Fixed,
    Zoom,
    Base
}
