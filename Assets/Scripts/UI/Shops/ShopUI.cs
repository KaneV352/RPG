using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace RPG.UI.Shops
{
  public class ShopUI : MonoBehaviour
  {
    [SerializeField] TextMeshProUGUI shopName;
    [SerializeField] Transform listRoot;
    [SerializeField] ShopItemUI shopItemUIPrefab;
    [SerializeField] TextMeshProUGUI totalField;
    [SerializeField] Button confirmButton;
    [SerializeField] Button switchButton;

    Shopper _shopper = null;
    Shop _currentShop = null;
    Color _originalTotalTextColor;
    // Start is called before the first frame update
    void Start()
    {
      _originalTotalTextColor = totalField.color;
      _shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
      if (_shopper == null) return;

      _shopper.OnShopChanged += ShopChanged;
      confirmButton.onClick.AddListener(ConfirmTransaction);
      switchButton.onClick.AddListener(SwitchMode);

      ShopChanged();
    }
    private void ShopChanged()
    {
      if (_currentShop != null)
      {
        _currentShop.OnChange -= RefreshUI;
      }
      _currentShop = _shopper.GetCurrentShop();
      gameObject.SetActive(_currentShop != null);

      foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
      {
        button.SetCurrentShop(_currentShop);
      }

      if (_currentShop == null) return;

      shopName.text = _currentShop.GetShopName();
      _currentShop.OnChange += RefreshUI;
      RefreshUI();
    }
    private void RefreshUI()
    {
      foreach (Transform child in listRoot)
      {
        Destroy(child.gameObject);
      }
      foreach (ShopItem item in _currentShop.GetFilteredItems())
      {
        ShopItemUI row = Instantiate<ShopItemUI>(shopItemUIPrefab, listRoot);
        row.Setup(item, _currentShop);
      }
      totalField.text = $"Total: ${_currentShop.TransactionTotal():N2}";
      confirmButton.interactable = _currentShop.CanTransact();
      TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
      TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
      if (_currentShop.IsBuyingMode())
      {
        totalField.color = _currentShop.HasSufficientFunds() ? _originalTotalTextColor : Color.red;
        switchText.text = "Switch To Selling";
        confirmText.text = "Buy";
      }
      else
      {
        totalField.color = _originalTotalTextColor;
        switchText.text = "Switch To Buying";
        confirmText.text = "Sell";
      }
    }

    public void Close()
    {
      _shopper.ExitShop();
    }
    public void ConfirmTransaction()
    {
      _currentShop.ConfirmTransaction();
    }

    public void SwitchMode()
    {
      _currentShop.SelectMode(!_currentShop.IsBuyingMode());
    }
  }
}