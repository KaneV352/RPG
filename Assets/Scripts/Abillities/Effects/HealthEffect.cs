using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Health Effect", menuName = "Abillities/Effects/Health Effect", order = 0)]
  public class HealthEffect : EffectStrategy
  {
    [FormerlySerializedAs("_amount")] [SerializeField] private float amount = 0;

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      foreach (var target in abillityData.GetTargets())
      {
        var targetHealth = target.GetComponent<Health>();
        if (!targetHealth) continue;

        if (amount > 0)
        {
          targetHealth.Heal(amount);
        }
        else
        {
          targetHealth.TakeDamage(abillityData.GetUser(), -amount);
        }
      }

      finished();
    }
  }
}