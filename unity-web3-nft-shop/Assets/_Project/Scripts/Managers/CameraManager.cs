using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera npcCamera;

    
    #region UNITY_LIFECYCLE

    private void OnEnable()
    {
        ShopCounterCollider.PlayerEnteredShop += ActivateNpcCamera;
        ShopInventory.Closed += ActivatePlayerCamera;
        PurchaseItemManager.PurchaseStarted += ActivatePlayerCamera;
    }

    private void OnDisable()
    {
        ShopCounterCollider.PlayerEnteredShop -= ActivateNpcCamera;
        ShopInventory.Closed -= ActivatePlayerCamera;
        PurchaseItemManager.PurchaseStarted -= ActivatePlayerCamera;
    }    

    #endregion


    #region EVENT_HANDLERS

    private void ActivateNpcCamera()
    {
        npcCamera.enabled = true;
        playerCamera.enabled = false;
    }
    
    private void ActivatePlayerCamera()
    {
        npcCamera.enabled = false;
        playerCamera.enabled = true;
    }

    #endregion
}
