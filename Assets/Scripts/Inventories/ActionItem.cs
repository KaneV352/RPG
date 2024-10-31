using System;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Inventories
{
  /// <summary>
  /// An inventory item that can be placed in the action bar and "Used".
  /// </summary>
  /// <remarks>
  /// This class should be used as a base. Subclasses must implement the `Use`
  /// method.
  /// </remarks>
  [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Action Item"))]
  public class ActionItem : InventoryItem
  {
    // CONFIG DATA
    [FormerlySerializedAs("_consumable")]
    [Tooltip("Does an instance of this item get consumed every time it's used.")]
    [SerializeField] private bool consumable = false;
    [FormerlySerializedAs("_cooldown")] [SerializeField] private float cooldown = 1f;

    protected CooldownStore CooldownStore;


    // PUBLIC

    /// <summary>
    /// Trigger the use of this item. Override to provide functionality.
    /// </summary>
    /// <param name="user">The character that is using this action.</param>
    public virtual void Use(GameObject user)
    {
      Debug.Log("Using action: " + this);
    }

    public bool IsConsumable()
    {
      return consumable;
    }

    public float GetCooldownTime()
    {
      return cooldown;
    }

    public float GetRemainingCooldown()
    {
      if (CooldownStore == null)
      {
        return 0;
      }
      return CooldownStore.GetRemainingCooldown(this);
    }

    public float GetCooldownFraction()
    {
      if (CooldownStore == null)
      {
        return 0;
      }
      return CooldownStore.GetCooldownFraction(this, cooldown);
    }
  }
}