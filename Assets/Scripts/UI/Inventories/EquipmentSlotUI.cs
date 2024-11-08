﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core.UI.Dragging;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

        // CACHE
        Equipment _playerEquipment;

        // LIFECYCLE METHODS
       
        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _playerEquipment = player.GetComponent<Equipment>();
            _playerEquipment.EquipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }

        // PUBLIC

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _playerEquipment.AddItem(equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return _playerEquipment.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            _playerEquipment.RemoveItem(equipLocation);
        }

        // PRIVATE

        void RedrawUI()
        {
            icon.SetItem(_playerEquipment.GetItemInSlot(equipLocation));
        }

        InventoryItem IItemHolder.GetItem()
        {
            return GetItem();
        }
    }
}