using MoralisUnity.Web3Api.Models;
using Pixelplacement;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public static string ContractAddress = "";
    public static string ContractAbi = "";
    public static ChainList ContractChain = ChainList.mumbai;
    
    public enum State
    {
        Authenticating,
        Free,
        Inventory,
        Transacting,
    }

    [SerializeField]
    private State _currentState;
    
    private GameInputActions _input;
    
    #region UNITY_LIFECYCLE

    private void Awake()
    {
        _currentState = State.Authenticating;

        _input = new GameInputActions();
        _input.General.Enable();
    }

    private void OnEnable()
    {
        MoralisAuthenticator.Authenticated += OnAuthenticationSuccessfulHandler;
        Inventory.Opened += OnInventoryOpenedHandler;
        Inventory.Closed += OnInventoryClosedHandler;
        
        PurchaseItemManager.PurchaseStarted += OnPurchaseStartedHandler;
        PurchaseItemManager.PurchaseCompleted += OnPurchaseCompletedHandler;
        PurchaseItemManager.PurchaseFailed += OnPurchaseFailedHandler;
        
        _input.General.Logout.performed += OnLogoutHandler;
        _input.General.Quit.performed += OnQuitHandler;
    }

    private void OnDisable()
    {
        MoralisAuthenticator.Authenticated -= OnAuthenticationSuccessfulHandler;
        Inventory.Opened += OnInventoryOpenedHandler;
        Inventory.Closed -= OnInventoryClosedHandler;
        
        PurchaseItemManager.PurchaseStarted -= OnPurchaseStartedHandler;
        PurchaseItemManager.PurchaseCompleted -= OnPurchaseCompletedHandler;
        PurchaseItemManager.PurchaseFailed -= OnPurchaseFailedHandler;
        
        _input.General.Logout.performed += OnLogoutHandler;
        _input.General.Quit.performed += OnQuitHandler;
    }

    #endregion


    #region PUBLIC_METHODS

    public State GetCurrentState()
    {
        return _currentState;
    }

    #endregion


    #region EVENT_HANDLERS

    private void OnAuthenticationSuccessfulHandler()
    {
        _currentState = State.Free;
    }
    
    private void OnInventoryOpenedHandler()
    {
        _currentState = State.Inventory;
    }

    private void OnInventoryClosedHandler()
    {
        _currentState = State.Free;
    }
    
    private void OnPurchaseStartedHandler()
    {
        _currentState = State.Transacting;
    }

    private void OnPurchaseCompletedHandler(string purchasedItemId)
    {
        _currentState = State.Free;
    }

    private void OnPurchaseFailedHandler()
    {
        _currentState = State.Free;
    }
    
    private void OnLogoutHandler(InputAction.CallbackContext context)
    {
        _currentState = State.Authenticating;
    }
    
    private void OnQuitHandler(InputAction.CallbackContext context)
    {
        Application.Quit();    
    }

    #endregion
}
