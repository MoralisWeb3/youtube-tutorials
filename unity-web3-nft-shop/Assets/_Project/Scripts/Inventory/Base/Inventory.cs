using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class Inventory : MonoBehaviour
{
    public static Action Opened;
    public static Action Closed;
    
    [Header("Item Prefab")]
    [SerializeField] protected InventoryItem item;
    
    [Header("UI Elements")]
    [SerializeField] protected GameObject uiPanel;
    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected GridLayoutGroup itemsGrid;
    [SerializeField] protected GameObject logo;
    
    private AudioSource _as;


    protected virtual void Awake()
    {
        _as = GetComponent<AudioSource>();
    }

    #region PRIVATE_METHODS

    protected void UpdateItem(string idToUpdate, ItemData newData)
    {
        foreach (Transform childItem in itemsGrid.transform)
        {
            InventoryItem item = childItem.GetComponent<InventoryItem>();

            if (item.GetId() == idToUpdate)
            {
                item.SetData(newData);
            }
        }
    }

    protected void ClearAllItems()
    {
        foreach (Transform childItem in itemsGrid.transform)
        {
            Destroy(childItem.gameObject);
        }
    }
    
    protected void DeleteItem(string idToDelete)
    {
        foreach (Transform childItem in itemsGrid.transform)
        {
            InventoryItem item = childItem.GetComponent<InventoryItem>();

            if (item.GetId() == idToDelete)
            {
                Destroy(item.gameObject);
            }
        }
    }
    
    protected void ActivatePanel(bool activate)
    {
        if (activate)
        {
            _as.Play();
        }
        
        uiPanel.SetActive(activate);

        if (logo == null) return;
        logo.SetActive(activate);
    }

    #endregion
    
    
    #region PUBLIC_METHODS

    public void OnCloseButtonClicked()
    {
        ClearAllItems();
        ActivatePanel(false);

        Closed?.Invoke();
    }

    #endregion
}
