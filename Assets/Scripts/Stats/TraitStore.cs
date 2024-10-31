using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
  public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable
  {
    [FormerlySerializedAs("_traitBonuses")] [SerializeField] private TraitBonus[] traitBonuses;

    [Serializable]
    public struct TraitBonus
    {
      public Trait trait;
      public TraitEffect[] traitEffects;
    }

    [Serializable]
    public struct TraitEffect
    {
      public Stat stat;
      [FormerlySerializedAs("AdictiveModifier")] public float adictiveModifier;
      [FormerlySerializedAs("PercentageModifier")] public float percentageModifier;
    }

    private Dictionary<Stat, Dictionary<Trait, float>> _additiveModifiersCache = new Dictionary<Stat, Dictionary<Trait, float>>();
    private Dictionary<Stat, Dictionary<Trait, float>> _percentageModifiersCache = new Dictionary<Stat, Dictionary<Trait, float>>();

    private Dictionary<Trait, int> _assignedPoints = new Dictionary<Trait, int>();
    private Dictionary<Trait, int> _stagedPoints = new Dictionary<Trait, int>();

    private int _usedPoints;

    private void OnEnable()
    {
      GetComponent<BaseStats>().OnLevelUp += CalculateUnassignedPoints;
    }

    private void OnDisable()
    {
      GetComponent<BaseStats>().OnLevelUp -= CalculateUnassignedPoints;
    }

    private void Awake()
    {
      PopulateCache();
    }

    private void PopulateCache()
    {
      foreach (TraitBonus traitBonus in traitBonuses)
      {
        foreach (TraitEffect traitEffect in traitBonus.traitEffects)
        {
          if (!_additiveModifiersCache.ContainsKey(traitEffect.stat))
          {
            _additiveModifiersCache[traitEffect.stat] = new Dictionary<Trait, float>();
          }
          if (!_percentageModifiersCache.ContainsKey(traitEffect.stat))
          {
            _percentageModifiersCache[traitEffect.stat] = new Dictionary<Trait, float>();
          }
          _additiveModifiersCache[traitEffect.stat][traitBonus.trait] = traitEffect.adictiveModifier;
          _percentageModifiersCache[traitEffect.stat][traitBonus.trait] = traitEffect.percentageModifier;
        }
      }
    }

    private void CalculateUnassignedPoints()
    {
      GetUnassignedPoints();
    }

    public int GetProposedPoints(Trait trait)
    {
      return GetPoints(trait) + GetStagedPoints(trait);
    }

    public int GetPoints(Trait trait)
    {
      return _assignedPoints.ContainsKey(trait) ? _assignedPoints[trait] : 0;
    }

    public int GetStagedPoints(Trait trait)
    {
      return _stagedPoints.ContainsKey(trait) ? _stagedPoints[trait] : 0;
    }

    public void AssignPoints(Trait trait, int points)
    {
      if (!CanAssignPoints(trait, points))
      {
        return;
      }

      _stagedPoints[trait] = GetStagedPoints(trait) + points;
      _usedPoints += points;
    }

    public bool CanAssignPoints(Trait trait, int points)
    {
      if (GetStagedPoints(trait) + points < 0)
      {
        return false;
      }
      if (GetUnassignedPoints() - points < 0)
      {
        return false;
      }
      return true;
    }

    public void CommitAssignment()
    {
      foreach (Trait trait in _stagedPoints.Keys)
      {
        _assignedPoints[trait] = GetProposedPoints(trait);
      }
      _stagedPoints.Clear();
    }

    public void CancelAssignment()
    {
      foreach (Trait trait in _stagedPoints.Keys)
      {
        _usedPoints -= _stagedPoints[trait];
      }
      _stagedPoints.Clear();
    }

    public int GetUnassignedPoints()
    {
      return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints) - _usedPoints;
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      if (!_additiveModifiersCache.ContainsKey(stat))
      {
        yield break;
      }
      foreach (Trait trait in _additiveModifiersCache[stat].Keys)
      {
        yield return _additiveModifiersCache[stat][trait] * GetPoints(trait);
      }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat)
    {
      if (!_percentageModifiersCache.ContainsKey(stat))
      {
        yield break;
      }
      foreach (Trait trait in _percentageModifiersCache[stat].Keys)
      {
        yield return _percentageModifiersCache[stat][trait] * GetPoints(trait);
      }
    }

    [Serializable]
    private struct SavingData
    {
      public Dictionary<Trait, int> AssignedPoints;
      [FormerlySerializedAs("UsedPoints")] public int usedPoints;
    }

    public object CaptureState()
    {
      SavingData savingData = new SavingData();

      CancelAssignment();
      savingData.AssignedPoints = _assignedPoints;
      savingData.usedPoints = _usedPoints;

      return savingData;
    }

    public void RestoreState(object state)
    {
      SavingData savingData = (SavingData)state;

      _assignedPoints = savingData.AssignedPoints;
      _usedPoints = savingData.usedPoints;
    }
  }
}