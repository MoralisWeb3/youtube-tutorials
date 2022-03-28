using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Main
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public static Action Dead;
        
        [Header("Controls")]
        [SerializeField] private Transform playerCameraT;
        [SerializeField] private float speed;
        [SerializeField] private float mouseSensitivity;

        [Header("HUD")] 
        [SerializeField] private Slider lifeSlider;
    
        private PlayerInput _playerInput;
        private CharacterController _characterController;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _focused;
        private float _xRotation = 0f;

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _characterController = GetComponent<CharacterController>();

            //We don't listen to any input until the user has logged in.
            _playerInput.DeactivateInput();
        }

        private void OnEnable()
        {
            EnemyRoot.PlayerAttacked += OnAttacked;
            MoralisWeb3Manager.LoggedInSuccessfully += OnUserLoggedIn;
        }

        private void OnDisable()
        {
            EnemyRoot.PlayerAttacked -= OnAttacked;
            MoralisWeb3Manager.LoggedInSuccessfully -= OnUserLoggedIn;
        }

        private void Update()
        {
            if (!_focused) return;
            
            //MOVEMENT
            Vector3 movementVector = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            _characterController.Move(movementVector * speed * Time.deltaTime);
            
            //CAMERA CONTROLS
            var mouseX = _lookInput.x * Time.deltaTime * mouseSensitivity;
            var mouseY = -_lookInput.y * Time.deltaTime * mouseSensitivity;

            _xRotation += mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            playerCameraT.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        #endregion

        #region PLAYER_LIFE

        private void OnAttacked()
        {
            lifeSlider.value -= 0.1f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlasmaSphere"))
            {
                lifeSlider.value -= 0.5f;
            }
            
            //If we collide with Boss, we're dead!
            if (other.gameObject.CompareTag("BossBody"))
            {
                lifeSlider.value -= 1;
            }
        }

        public void HandleSliderValue()
        {
            // If LIFE = 0, WE DEAD.
            if (Mathf.Approximately(lifeSlider.value, 0))
            {
                _playerInput.DeactivateInput();
                HandleUnfocus();
                Dead?.Invoke();
            }
        }

        #endregion

        #region INPUT_ACTION_HANDLERS

        public void HandleFocus()
        {
            if (_focused) return;
        
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _focused = true;
        }

        public void HandleUnfocus()
        {
            if (!_focused) return;
        
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _focused = false;
        }

        public void HandleMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void HandleLook(InputAction.CallbackContext context)
        {
            if (!_focused) return;
            
            _lookInput = context.ReadValue<Vector2>();
        }

        public void HandleSprint(InputAction.CallbackContext context)
        {
            const int multiplier = 2;
            
            if (context.performed)
            {
                speed *= multiplier;
            }

            if (context.canceled)
            {
                speed /= multiplier;
            }
        }

        #endregion

        #region EVENT_HANDLERS

        private void OnUserLoggedIn()
        {
            _playerInput.ActivateInput();
        }

        #endregion
    }   
}
