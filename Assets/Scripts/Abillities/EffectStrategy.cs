using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abillities
{

  public abstract class EffectStrategy : ScriptableObject
  {
    public abstract void StartEffect(AbillityData abillityData, Action finished);
  }
}