using System.Collections;
using System.Collections.Generic;
using InternalAssets.Scripts.Services;
using UnityEngine;

namespace HolmanPlayerController
{
    public class CameraFree : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] private Transform transform;
        [SerializeField] float sensitivity;
        [SerializeField] private float lowestAngle;
        [SerializeField] private float highestAngle;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            sensitivity = StaticSettingsService.sensitivity;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            
            if(Cursor.lockState == CursorLockMode.None)
                return;
                
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
            transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0);
            player.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);

            var forward = transform.forward;
            var modifiedForward = new Vector3(forward.x, 0, forward.z);

            var angle = Vector3.SignedAngle(modifiedForward, forward, transform.right);
            
            if (angle < -highestAngle)
                transform.localRotation = Quaternion.Euler(Vector3.right * -highestAngle);
            
            if (angle > lowestAngle)
                transform.localRotation = Quaternion.Euler(Vector3.right * lowestAngle);
        }
    }
}
