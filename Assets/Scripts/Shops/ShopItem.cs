using System;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
  public class ShopItem
  {
    InventoryItem _item;
    int _availability;
    float _price;
    int _quantityInTransaction;

    public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction = 0)
    {
      this._item = item;
      this._availability = availability;
      this._price = price;
      this._quantityInTransaction = quantityInTransaction;
    }

    public InventoryItem GetInventoryItem()
    {
      return _item;
    }

    public int GetAvailability()
    {
      return _availability;
    }

    public float GetPrice()
    {
      return _price;
    }

    public int GetQuantityInTransaction()
    {
      return _quantityInTransaction;
    }
  }
}