using RPG.Core.UI.Dragging;
using RPG.Inventories;
using RPG.Abillities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
  /// <summary>
  /// The UI slot for the player action bar.
  /// </summary>
  public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<RPG.Inventories.InventoryItem>
  {
    // CONFIG DATA
    [FormerlySerializedAs("_icon")] [SerializeField] private InventoryItemIcon icon = null;
    [FormerlySerializedAs("_index")] [SerializeField] private int index = 0;
    [FormerlySerializedAs("_cooldownImage")] [SerializeField] private Image cooldownImage = null;

    // CACHE
    private ActionStore _store;

    // LIFECYCLE METHODS
    private void Awake()
    {
      _store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
      _store.StoreUpdated += UpdateIcon;
    }

    private void Update()
    {
      if (_store.GetAction(index) == null) return;

      var item = _store.GetAction(index);
      if (item == null)
      {
        cooldownImage.fillAmount = 0;
        return;
      }

      var cooldownFraction = item.GetCooldownFraction();

      cooldownImage.fillAmount = cooldownFraction;
    }

    // PUBLIC

    public void AddItems(RPG.Inventories.InventoryItem item, int number)
    {
      _store.AddAction(item, index, number);
    }

    public RPG.Inventories.InventoryItem GetItem()
    {
      return _store.GetAction(index);
    }

    public int GetNumber()
    {
      return _store.GetNumber(index);
    }

    public int MaxAcceptable(RPG.Inventories.InventoryItem item)
    {
      return _store.MaxAcceptable(item, index);
    }

    public void RemoveItems(int number)
    {
      _store.RemoveItems(index, number);
    }

    // PRIVATE

    void UpdateIcon()
    {
      icon.SetItem(GetItem(), GetNumber());
    }
  }
}
