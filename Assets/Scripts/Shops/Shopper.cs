using System;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
  public class Shopper : MonoBehaviour
  {
    Shop _currentShop = null;
    public Action OnShopChanged;

    public void SetCurrentShop(Shop shop)
    {
      if (_currentShop != null)
      {
        _currentShop.SetShopper(null);
      }
      _currentShop = shop;
      _currentShop.SetShopper(this);
      _currentShop.SelectFilter(ItemFilter.None);

      if (OnShopChanged != null)
      {
        OnShopChanged();
      }
    }

    public Shop GetCurrentShop()
    {
      return _currentShop;
    }

    public void ExitShop()
    {
      _currentShop = null;

      if (OnShopChanged != null)
      {
        OnShopChanged();
      }
    }
  }
}
