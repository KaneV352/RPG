using System;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Inventories
{
  public class Purse : MonoBehaviour, ISaveable
  {
    [FormerlySerializedAs("_initialBalance")] [SerializeField] private float initialBalance = 100;

    private float _balance = 0;

    public Action OnBalanceChanged;

    public float GetBalance()
    {
      return _balance;
    }

    private void Awake()
    {
      _balance = initialBalance;
    }

    public void UpdateBalance(float amount)
    {
      _balance += amount;

      OnBalanceChanged?.Invoke();
    }

    public bool CanAfford(float amount)
    {
      return _balance >= amount;
    }

    public object CaptureState()
    {
      return _balance;
    }

    public void RestoreState(object state)
    {
      _balance = (float)state;
    }
  }
}