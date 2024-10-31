using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Mana Effect", menuName = "Abillities/Effects/Mana Effect", order = 0)]
  public class ManaEffect : EffectStrategy
  {
    [FormerlySerializedAs("_amount")] [SerializeField] private float amount = 0;

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      foreach (var target in abillityData.GetTargets())
      {
        Mana targetMana = target.GetComponent<Mana>();
        if (!targetMana) continue;

        if (amount > 0)
        {
          targetMana.RecoverMana(amount);
        }
        else
        {
          targetMana.RecoverMana(-amount);
        }
      }

      finished();
    }
  }
}