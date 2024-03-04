using UnityEngine;

namespace InternalAssets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterController _characterController;

        private Vector3 _cameraForwardVector;
        private Vector3 _cameraRightVector;
        
        void Start()
        {
            _cameraForwardVector = _camera.transform.forward;
            _cameraRightVector = _camera.transform.right;
        }

        // Update is called once per frame
        void Update()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            
            var cameraForwardVector = _camera.transform.forward;
            var cameraRightVector = _camera.transform.right;
            
            cameraForwardVector.y = 0;
            cameraForwardVector.Normalize();
            
            cameraRightVector.y = 0;
            cameraRightVector.Normalize();

            _characterController.Move(2 * Time.deltaTime *
                                      (verticalAxis * cameraForwardVector + horizontalAxis * cameraRightVector).normalized);
            _characterController.Move(9.8f * Time.deltaTime * Vector3.down);
            //_rigidbody.AddForce(verticalAxis * cameraForwardVector + horizontalAxis * cameraRightVector, ForceMode.VelocityChange);
        }
    }
}
