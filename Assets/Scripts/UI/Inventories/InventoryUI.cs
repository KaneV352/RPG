using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using UnityEngine.Serialization;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [FormerlySerializedAs("InventoryItemPrefab")] [SerializeField] InventorySlotUI inventoryItemPrefab = null;

        // CACHE
        Inventory _playerInventory;

        // LIFECYCLE METHODS

        private void Awake() 
        {
            _playerInventory = Inventory.GetPlayerInventory();
            _playerInventory.InventoryUpdated += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        // PRIVATE

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(inventoryItemPrefab, transform);
                itemUI.Setup(_playerInventory, i);
            }
        }
    }
}