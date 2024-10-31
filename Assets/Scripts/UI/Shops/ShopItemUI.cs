using System;
using RPG.Shops;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  public class ShopItemUI : MonoBehaviour
  {
    [FormerlySerializedAs("_itemName")] [SerializeField] private TMPro.TextMeshProUGUI itemName = null;
    [FormerlySerializedAs("_price")] [SerializeField] private TMPro.TextMeshProUGUI price = null;
    [FormerlySerializedAs("_availability")] [SerializeField] private TMPro.TextMeshProUGUI availability = null;
    [FormerlySerializedAs("_quantity")] [SerializeField] private TMPro.TextMeshProUGUI quantity = null;
    [FormerlySerializedAs("_image")] [SerializeField] private Image image = null;

    private Shop _shop;
    private ShopItem _shopItem;

    public void Setup(ShopItem shopItem, Shop shop)
    {
      _shopItem = shopItem;
      itemName.text = _shopItem.GetInventoryItem().GetDisplayName();
      price.text = $"${_shopItem.GetPrice():N2}";
      availability.text = _shopItem.GetAvailability().ToString();
      quantity.text = $"{_shopItem.GetQuantityInTransaction().ToString()}";
      image.sprite = _shopItem.GetInventoryItem().GetIcon();

      _shop = shop;
    }

    public void Add()
    {
      _shop.AddToTransaction(_shopItem.GetInventoryItem(), 1);
    }

    public void Subtract()
    {
      _shop.AddToTransaction(_shopItem.GetInventoryItem(), -1);
    }
  }
}