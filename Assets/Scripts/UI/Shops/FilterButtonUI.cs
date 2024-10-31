using RPG.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  public class FilterButtonUI : MonoBehaviour
  {
    [FormerlySerializedAs("_category")] [SerializeField] ItemFilter category;

    Button _button;
    Shop _currentShop;

    private void Awake()
    {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(SelectFilter);
    }

    private void Start()
    {
      RefreshUI();
    }

    public void SetCurrentShop(Shop currentShop)
    {
      if (_currentShop != null)
      {
        _currentShop.OnChange -= RefreshUI;
      }

      _currentShop = currentShop;

      if (_currentShop != null)
      {
        _currentShop.OnChange += RefreshUI;
      }

      RefreshUI();
    }

    public void RefreshUI()
    {
      if (_currentShop == null) return;
      _button.interactable = _currentShop.GetFilter() != category;
    }

    public void SelectFilter()
    {
      _currentShop.SelectFilter(category);
    }
  }
}