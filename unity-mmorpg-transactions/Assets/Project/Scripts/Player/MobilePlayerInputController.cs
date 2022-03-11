using UnityEngine;
using UnityEngine.InputSystem;

namespace Web3MultiplayerRPG
{
    public class MobilePlayerInputController : MonoBehaviour
    {
        private PlayerInputController _playerInputController;
        private PlayerInput _playerInput;

        public void SetInputController(PlayerInputController newPlayerInputController, PlayerInput newPlayerInput)
        {
            _playerInputController = newPlayerInputController;

            _playerInput = newPlayerInput;
            _playerInput.neverAutoSwitchControlSchemes = true;
        }
        
        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            _playerInputController.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            _playerInputController.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            _playerInputController.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            _playerInputController.SprintInput(virtualSprintState);
        }
    }
}
