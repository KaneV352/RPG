using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories;
using RPG.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Shops
{
  public class Shop : MonoBehaviour, IRaycastable, ISaveable
  {
    [SerializeField] string shopName;
    [Range(0, 100)]
    [SerializeField] float sellingPercentage = 20f;
    [SerializeField] float maximumBarterBonus = 20f;
    [SerializeField] float maximumBuyingDistance = 1f;

    [FormerlySerializedAs("_stockConfig")] [SerializeField]
    StockItemConfig[] stockConfig;
    [System.Serializable]
    class StockItemConfig
    {
      public InventoryItem item;
      public int initialStock;
      [Range(0, 100)]
      public float buyingDiscountPercentage;
      public int levelToUnlock = 0;
    }
    Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
    Dictionary<InventoryItem, int> _stockSold = new Dictionary<InventoryItem, int>();
    Shopper _currentShopper = null;
    bool _isBuyingMode = true;
    ItemFilter _currentFilter = ItemFilter.None;

    public event Action OnChange;

    public void SetShopper(Shopper shopper)
    {
      _currentShopper = shopper;
    }
    public IEnumerable<ShopItem> GetFilteredItems()
    {
      var items = GetAllItems();

      if (_currentFilter == ItemFilter.None)
      {
        return items;
      }

      return items.Where(item => item.GetInventoryItem().GetCategory() == _currentFilter);
    }
    public IEnumerable<ShopItem> GetAllItems()
    {
      Dictionary<InventoryItem, float> prices = GetPrices();
      Dictionary<InventoryItem, int> availabilities = GetAvailabilities();

      foreach (InventoryItem item in availabilities.Keys)
      {
        if (availabilities[item] <= 0) continue;

        float price = prices[item];
        int quantityInTransaction = 0;

        _transaction.TryGetValue(item, out quantityInTransaction);
        int availability = availabilities[item];
        yield return new ShopItem(item, availability, price, quantityInTransaction);
      }
    }

    public void SelectFilter(ItemFilter filter)
    {
      _currentFilter = filter;

      if (OnChange != null)
      {
        OnChange();
      }
    }
    public ItemFilter GetFilter()
    {
      return _currentFilter;
    }

    public void SelectMode(bool isBuying)
    {
      _isBuyingMode = isBuying;
      if (OnChange != null)
      {
        OnChange();
      }
    }

    public bool IsBuyingMode()
    {
      return _isBuyingMode;
    }

    public bool CanTransact()
    {
      if (_currentShopper == null) return false;
      if (IsTransactionEmpty()) return false;
      if (!_isBuyingMode) return true;
      if (!HasSufficientFunds()) return false;
      if (!HasInventorySpace()) return false;
      return true;
    }
    public bool HasSufficientFunds()
    {
      Purse purse = _currentShopper.GetComponent<Purse>();
      if (purse == null) return false;
      return purse.GetBalance() >= TransactionTotal();
    }
    public bool IsTransactionEmpty()
    {
      return _transaction.Count == 0;
    }
    public bool HasInventorySpace()
    {
      Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
      if (shopperInventory == null) return false;
      List<InventoryItem> flatItems = new List<InventoryItem>();
      foreach (ShopItem shopItem in GetAllItems())
      {
        InventoryItem item = shopItem.GetInventoryItem();
        int quantity = shopItem.GetQuantityInTransaction();
        for (int i = 0; i < quantity; i++)
        {
          flatItems.Add(item);
        }
      }
      return shopperInventory.HasSpaceFor(flatItems);
    }

    public void ConfirmTransaction()
    {
      Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
      Purse shopperPurse = _currentShopper.GetComponent<Purse>();
      if (shopperInventory == null || shopperPurse == null) return;

      foreach (ShopItem shopItem in GetAllItems())
      {
        InventoryItem item = shopItem.GetInventoryItem();
        int quantity = shopItem.GetQuantityInTransaction();
        float price = shopItem.GetPrice();
        for (int i = 0; i < quantity; i++)
        {
          if (_isBuyingMode)
          {
            BuyItem(shopperInventory, shopperPurse, item, price);
          }
          else
          {
            SellItem(shopperInventory, shopperPurse, item, price);
          }
        }
      }

      if (OnChange != null)
      {
        OnChange();
      }
    }

    public float TransactionTotal()
    {
      float total = 0;
      foreach (ShopItem item in GetAllItems())
      {
        total += item.GetPrice() * item.GetQuantityInTransaction();
      }
      return total;
    }
    public string GetShopName()
    {
      return shopName;
    }
    public void AddToTransaction(InventoryItem item, int quantity)
    {
      if (!_transaction.ContainsKey(item))
      {
        _transaction[item] = 0;
      }

      var availabilities = GetAvailabilities();
      if (_transaction[item] + quantity > availabilities[item])
      {
        _transaction[item] = availabilities[item];
      }
      else
      {
        _transaction[item] += quantity;
      }

      if (_transaction[item] <= 0)
      {
        _transaction.Remove(item);
      }


      if (OnChange != null)
      {
        OnChange();
      }
    }
    public CursorType GetCursorType()
    {
      return CursorType.Shop;
    }
    public bool HandleRaycast(PlayerController callingController)
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (Vector3.Distance(callingController.transform.position, transform.position) > maximumBuyingDistance)
        {
          StartCoroutine(GetInRangeToBuy(callingController));
        }
        else
        {
          callingController.GetComponent<Shopper>().SetCurrentShop(this);
        }
      }
      return true;
    }

    private IEnumerator GetInRangeToBuy(PlayerController callingController)
    {
      callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
      yield return new WaitUntil(() => Vector3.Distance(callingController.transform.position, transform.position) <= maximumBuyingDistance);

      callingController.GetComponent<Shopper>().SetCurrentShop(this);
    }

    private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
    {
      int slot = shopperInventory.FindFirstItem(item);
      if (slot < 0) return;

      shopperInventory.RemoveFromSlot(slot, 1);
      AddToTransaction(item, -1);
      if (_stockSold.ContainsKey(item))
      {
        _stockSold[item]--;
        if (_stockSold[item] <= 0)
        {
          _stockSold.Remove(item);
        }
      }
      shopperPurse.UpdateBalance(price);
    }

    private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
    {
      if (shopperPurse.GetBalance() < price) return;

      bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
      if (success)
      {
        AddToTransaction(item, -1);
        if (_stockSold.ContainsKey(item))
        {
          _stockSold[item]++;
        }
        else
        {
          _stockSold[item] = 1;
        }
        shopperPurse.UpdateBalance(-price);
      }
    }

    private int CountItemsInInventory(InventoryItem item)
    {
      int count = 0;
      Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
      for (int i = 0; i < shopperInventory.GetSize(); i++)
      {
        if (object.ReferenceEquals(shopperInventory.GetItemInSlot(i), item))
        {
          count += shopperInventory.GetNumberInSlot(i);
        }
      }

      return count;
    }

    private int GetShopperLevel()
    {
      return _currentShopper.GetComponent<BaseStats>().GetLevel();
    }

    private IEnumerable<StockItemConfig> GetAvailableConfigs()
    {
      int shopperLevel = GetShopperLevel();
      foreach (var config in stockConfig)
      {
        if (config.levelToUnlock > shopperLevel) continue;
        StockItemConfig newConfig = new StockItemConfig
        {
          item = config.item,
          initialStock = config.initialStock * shopperLevel,
          buyingDiscountPercentage = config.buyingDiscountPercentage * shopperLevel,
        };

        yield return newConfig;
      }
    }

    private Dictionary<InventoryItem, int> GetAvailabilities()
    {
      Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

      foreach (var config in GetAvailableConfigs())
      {
        if (_isBuyingMode)
        {
          if (!availabilities.ContainsKey(config.item))
          {
            _stockSold.TryGetValue(config.item, out int sold);
            availabilities[config.item] = -sold;
          }
          availabilities[config.item] += config.initialStock;
        }
        else
        {
          availabilities[config.item] = CountItemsInInventory(config.item);
        }
      }

      return availabilities;
    }

    private Dictionary<InventoryItem, float> GetPrices()
    {
      Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

      foreach (var config in GetAvailableConfigs())
      {
        if (_isBuyingMode)
        {
          if (!prices.ContainsKey(config.item))
          {
            prices[config.item] = config.item.GetPrice();
          }
          prices[config.item] *= (1 - config.buyingDiscountPercentage / 100) * GetBarterBonus();
        }
        else
        {
          if (!prices.ContainsKey(config.item))
          {
            prices[config.item] = config.item.GetPrice();
          }
          prices[config.item] *= 1 - sellingPercentage / 100;
        }
      }

      return prices;
    }

    private float GetBarterBonus()
    {
      BaseStats baseStats = _currentShopper.GetComponent<BaseStats>();
      if (baseStats == null) return 1;

      float discount = baseStats.GetStat(Stat.BarterBonus);
      return 1 - Mathf.Min(discount, maximumBarterBonus) / 100;
    }

    public object CaptureState()
    {
      Dictionary<string, int> stockSold = new Dictionary<string, int>();

      foreach (var item in _stockSold)
      {
        stockSold[item.Key.GetItemID()] = item.Value;
      }

      return stockSold;
    }

    public void RestoreState(object state)
    {
      Dictionary<string, int> stockSold = (Dictionary<string, int>)state;

      _stockSold.Clear();
      foreach (var item in stockSold)
      {
        var inventoryItem = InventoryItem.GetFromID(item.Key);
        _stockSold[inventoryItem] = item.Value;
      }
    }
  }
}