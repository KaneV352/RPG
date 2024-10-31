using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Inventories
{
  public class CooldownStore : MonoBehaviour
  {
    private Dictionary<InventoryItem, float> _cooldownTimers = new Dictionary<InventoryItem, float>();

    private void Update()
    {
      var keys = new List<InventoryItem>(_cooldownTimers.Keys);

      foreach (var item in keys)
      {
        if (_cooldownTimers[item] <= 0 || !_cooldownTimers.ContainsKey(item))
        {
          continue;
        }

        _cooldownTimers[item] -= Time.deltaTime;
        if (_cooldownTimers[item] <= 0)
        {
          _cooldownTimers.Remove(item);
        }
      }
    }

    public void StartCooldown(InventoryItem item, float cooldown)
    {
      if (_cooldownTimers.ContainsKey(item))
      {
        _cooldownTimers[item] = cooldown;
      }
      else
      {
        _cooldownTimers.Add(item, cooldown);
      }
    }

    public bool IsOnCooldown(InventoryItem item)
    {
      if (_cooldownTimers.ContainsKey(item))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public float GetRemainingCooldown(InventoryItem item)
    {
      if (!_cooldownTimers.ContainsKey(item))
      {
        return 0;
      }

      return _cooldownTimers[item];
    }

    public float GetCooldownFraction(InventoryItem item, float cooldown)
    {

      return GetRemainingCooldown(item) / cooldown;
    }
  }


}