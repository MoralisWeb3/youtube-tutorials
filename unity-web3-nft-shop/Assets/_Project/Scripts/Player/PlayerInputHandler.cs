using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	[HideInInspector] public Vector2 moveInput;
	[HideInInspector] public Vector2 lookInput;

	private GameInputActions _input;

	#region UNITY_LIFECYCLE

	private void Awake()
	{
		_input = new GameInputActions();
		_input.General.Enable();
	}

	private void OnEnable()
	{
		_input.Player.Move.performed += OnMoveHandler;
		_input.Player.Look.performed += OnLookHandler;

		_input.General.Logout.performed += DeactivateInput;

		MoralisAuthenticator.Authenticated += ActivateInput;
		PurchaseItemManager.PurchaseStarted += ActivateInput;
		
		Inventory.Opened += DeactivateInput;
		Inventory.Closed += ActivateInput;
	}

	private void OnDisable()
	{
		_input.Player.Move.performed -= OnMoveHandler;
		_input.Player.Look.performed -= OnLookHandler;
		
		_input.General.Logout.performed -= DeactivateInput;
		
		MoralisAuthenticator.Authenticated -= ActivateInput;
		PurchaseItemManager.PurchaseStarted -= ActivateInput;
		
		Inventory.Opened += DeactivateInput;
		Inventory.Closed += ActivateInput;
	}

	#endregion


	#region INPUT_SYSTEM_EVENT_HANDLERS

	private void OnMoveHandler(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}
	
	private void OnLookHandler(InputAction.CallbackContext context)
	{
		lookInput = context.ReadValue<Vector2>();
	}

	#endregion


	#region PRIVATE_METHODS

	private void ActivateInput()
	{
		_input.Player.Enable();
		LockCursor(true);
	}

	private void DeactivateInput()
	{
		_input.Player.Disable();
		moveInput = Vector2.zero;
		lookInput = Vector2.zero;
		LockCursor(false);
	}
	
	private void DeactivateInput(InputAction.CallbackContext context)
	{
		_input.Player.Disable();
		moveInput = Vector2.zero;
		lookInput = Vector2.zero;
		LockCursor(false);
	}

	private static void LockCursor(bool isTrue)
	{
		Cursor.lockState = isTrue ? CursorLockMode.Locked : CursorLockMode.None;
	}

	#endregion
}