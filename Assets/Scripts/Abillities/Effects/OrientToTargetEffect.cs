using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Orient To Target Effect", menuName = "Abillities/Effects/Orient To Target Effect", order = 0)]
  public class OrientToTargetEffect : EffectStrategy
  {
    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      var transform = abillityData.GetUser().GetComponent<Transform>();
      transform.LookAt(abillityData.GetTargetPoint());

      finished();
    }
  }
}