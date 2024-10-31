using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "Abillities/Effects/Trigger Animation Effect", order = 0)]
  public class TriggerAnimationEffect : EffectStrategy
  {
    [FormerlySerializedAs("_animationTrigger")] [SerializeField] private string animationTrigger = "";

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      var animator = abillityData.GetUser().GetComponent<Animator>();
      animator.SetTrigger(animationTrigger);

      finished();
    }
  }
}