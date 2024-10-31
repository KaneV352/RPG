using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abillities.Targeting
{
  [CreateAssetMenu(fileName = "Self Targeting", menuName = "Abillities/Targeting/SelfT argeting", order = 0)]
  public class SelfTargeting : TargetingStrategy
  {
    public override void StartTargeting(AbillityData abillityData, Action finished)
    {
      GameObject user = abillityData.GetUser();
      abillityData.SetTargets(new GameObject[] { user });
      abillityData.SetTargetPoint(user.transform.position);

      finished();
    }
  }
}