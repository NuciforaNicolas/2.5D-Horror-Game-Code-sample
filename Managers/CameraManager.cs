using System;
using Cinemachine;
using Managers.SO;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraEventsChannelSO cameraEventsChannelSo;
        
        [Space]
        [SerializeField] private CinemachineVirtualCamera baseCam;
        [SerializeField] private CinemachineVirtualCamera zoomCam;
        [SerializeField] private CinemachineVirtualCamera dialogueCam;
        [SerializeField] private CinemachineVirtualCamera fixedCam;

        // Used to access body properties of the cinemachine camera
        private CinemachineFramingTransposer _zoomCamTransposer;
        
        private Transform _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;

            _zoomCamTransposer = zoomCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            
            // Subscribe events
            cameraEventsChannelSo.OnDialogueStart += UseDialogueCamera;
            cameraEventsChannelSo.OnDialogueEnd += DismissDialogueCamera;
            cameraEventsChannelSo.OnEnterFixedRoom += UseFixedCamera;
            cameraEventsChannelSo.OnEnterZoomRoom += UseZoomCamera;
            cameraEventsChannelSo.OnEnterBaseRoom += UseBaseCamera;
        }

        private void OnDestroy()
        {
            // Unsubscribe events
            cameraEventsChannelSo.OnDialogueStart -= UseDialogueCamera;
            cameraEventsChannelSo.OnDialogueEnd -= DismissDialogueCamera;
            cameraEventsChannelSo.OnEnterFixedRoom -= UseFixedCamera;
            cameraEventsChannelSo.OnEnterZoomRoom -= UseZoomCamera;
            cameraEventsChannelSo.OnEnterBaseRoom -= UseBaseCamera;
        }

        private void ResetPriorities()
        {
            baseCam.Priority = 10;
            zoomCam.Priority = 10;
            fixedCam.Priority = 10;
            dialogueCam.Priority = 10;
        }
        
        /// <summary>
        /// Requested the dialogue camera
        /// </summary>
        /// <param name="t">Transform to look at during the dialogue</param>
        private void UseDialogueCamera(Transform t)
        {
            dialogueCam.LookAt = t;

            dialogueCam.Priority = 100;
        }

        private void DismissDialogueCamera()
        {
            dialogueCam.Priority = 10;

            dialogueCam.LookAt = null;
        }

        /// <summary>
        /// Requested fixed camera
        /// </summary>
        /// <param name="pos">Position of the camera</param>
        /// <param name="rot">Rotation of the camera</param>
        /// <param name="lookAt">If true look at the player</param>
        private void UseFixedCamera(Vector3 pos, Quaternion rot, bool lookAt)
        {
            fixedCam.LookAt = lookAt ? _player : null;
            fixedCam.transform.position = pos;
            if (!lookAt)
            {
                fixedCam.transform.rotation = rot;
            }
            
            ResetPriorities();
            fixedCam.Priority = 11;
        }
        
        /// <summary>
        /// Requested zoom camera
        /// </summary>
        /// <param name="dist">Distance property</param>
        private void UseZoomCamera(float dist)
        {
            _zoomCamTransposer.m_CameraDistance = dist;
            
            ResetPriorities();
            zoomCam.Priority = 11;
        }
        
        /// <summary>
        /// Requested base camera
        /// </summary>
        private void UseBaseCamera()
        {
            ResetPriorities();
            baseCam.Priority = 11;
        }
    }
}
