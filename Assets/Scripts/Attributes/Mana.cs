using System;
using RPG.Saving;
using RPG.Utils;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Attributes
{
  public class Mana : MonoBehaviour, ISaveable
  {
    [FormerlySerializedAs("_regenerationPercentage")] [SerializeField] private float regenerationPercentage = 70;

    private LazyValue<float> _currentMana;

    private void Awake()
    {
      _currentMana = new LazyValue<float>(GetInitialMana);
    }

    private void Start()
    {
      _currentMana.ForceInit();
    }

    private void Update()
    {
      _currentMana.value = Mathf.Min(_currentMana.value + GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate) * Time.deltaTime, GetInitialMana());
    }

    private void OnEnable()
    {
      GetComponent<BaseStats>().OnLevelUp += RegenerateMana;
    }

    private void RegenerateMana()
    {
      float manaRegen = GetComponent<BaseStats>().GetStat(Stat.Mana) * regenerationPercentage / 100;
      _currentMana.value = Mathf.Max(_currentMana.value + manaRegen, GetInitialMana());
    }

    private void OnDisable()
    {
      GetComponent<BaseStats>().OnLevelUp -= RegenerateMana;
    }

    public float GetInitialMana()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Mana);
    }

    public float GetMana()
    {
      return _currentMana.value;
    }

    public void UseMana(float amount)
    {
      _currentMana.value = Mathf.Max(_currentMana.value - amount, 0);
    }

    public void RecoverMana(float amount)
    {
      UseMana(-amount);
    }

    public object CaptureState()
    {
      return _currentMana.value;
    }

    public void RestoreState(object state)
    {
      _currentMana.value = (float)state;
    }
  }
}