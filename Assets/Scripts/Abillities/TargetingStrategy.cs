using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abillities
{

  public abstract class TargetingStrategy : ScriptableObject
  {
    public abstract void StartTargeting(AbillityData abillityData, Action finished);
  }
}