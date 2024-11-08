﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
  [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
  public class Progression : ScriptableObject
  {
    [SerializeField] ProgressionCharacterClass[] characterClasses = null;

    Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

    public float GetStat(Stat stat, CharacterClass characterClass, int level)
    {
      BuildLookup();

      if (!_lookupTable[characterClass].ContainsKey(stat)) return 0;

      float[] levels = _lookupTable[characterClass][stat];

      if (levels.Length == 0) return 0;
      if (levels.Length < level)
      {
        return levels[levels.Length - 1];
      }

      return levels[level - 1];
    }

    public int GetLevels(Stat stat, CharacterClass characterClass)
    {
      BuildLookup();

      float[] levels = _lookupTable[characterClass][stat];
      return levels.Length;
    }

    private void BuildLookup()
    {
      if (_lookupTable != null) return;

      _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

      foreach (ProgressionCharacterClass progressionClass in characterClasses)
      {
        var statLookupTable = new Dictionary<Stat, float[]>();

        foreach (ProgressionStat progressionStat in progressionClass.stats)
        {
          statLookupTable[progressionStat.stat] = progressionStat.levels;
        }

        _lookupTable[progressionClass.characterClass] = statLookupTable;
      }
    }

    [Serializable]
    class ProgressionCharacterClass
    {
      public CharacterClass characterClass;
      public ProgressionStat[] stats;
    }

    [Serializable]
    class ProgressionStat
    {
      public Stat stat;
      public float[] levels;
    }
  }
}