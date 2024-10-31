using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abillities/Effects/Delay Composite Effect", order = 0)]
  public class DelayCompositeEffect : EffectStrategy
  {
    [FormerlySerializedAs("_effects")] [SerializeField] private EffectStrategy[] effects = null;
    [FormerlySerializedAs("_delay")] [SerializeField] private float delay = 0;
    [FormerlySerializedAs("_shouldAbortOnCancellation")] [SerializeField] private bool shouldAbortOnCancellation = false;

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      abillityData.StartCoroutine(DelayEffect(abillityData, finished));
    }

    private IEnumerator DelayEffect(AbillityData abillityData, Action finished)
    {
      yield return new WaitForSeconds(delay);
      if (shouldAbortOnCancellation && abillityData.IsCancelled())
      {
        yield break;
      }

      foreach (var effect in effects)
      {
        effect.StartEffect(abillityData, finished);
      }
      finished();

    }
  }
}