using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    public class MyInputsManager : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                _cameraController.SetMovementDirection(context.ReadValue<Vector2>());
            else if (context.canceled)
                _cameraController.SetMovementDirection(Vector2.zero);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
                _cameraController.SetSpeed(true);
            else if (context.canceled)
                _cameraController.SetSpeed(false);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.performed)
                _cameraController.SetRotationDirection(context.ReadValue<Vector2>());
            else if (context.canceled)
                _cameraController.SetRotationDirection(Vector2.zero);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _cameraController.SetZoomDirection(context.ReadValue<float>());
            }
            else if (context.canceled)
                _cameraController.SetZoomDirection(0);
        }
    }
}