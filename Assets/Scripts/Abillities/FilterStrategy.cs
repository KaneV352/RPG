using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abillities
{

  public abstract class FilterStrategy : ScriptableObject
  {
    public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectToFilters);
  }
}