using RPG.Inventories;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities
{
  [CreateAssetMenu(fileName = "My Abillity", menuName = "Abillities/Abillity", order = 0)]
  public class InventoryItem : ActionItem
  {
    [FormerlySerializedAs("_targetingStrategy")] [SerializeField] private TargetingStrategy targetingStrategy = null;
    [FormerlySerializedAs("_filterStrategy")] [SerializeField] private FilterStrategy[] filterStrategy = null;
    [FormerlySerializedAs("_effectStrategy")] [SerializeField] private EffectStrategy[] effectStrategy = null;
    [FormerlySerializedAs("_manaCost")] [SerializeField] private float manaCost = 0;

    private Mana _mana;

    public override void Use(GameObject user)
    {
      CooldownStore = user.GetComponent<CooldownStore>();
      _mana = user.GetComponent<Mana>();
      if (!HasEnoughMana())
      {
        return;
      }

      if (CooldownStore.IsOnCooldown(this))
      {
        return;
      }

      AbillityData abillityData = new AbillityData(user);
      ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
      actionScheduler.StartAction(abillityData);
      abillityData.StartAction();

      targetingStrategy.StartTargeting(abillityData,
        () => TargetAcquired(abillityData));
    }

    private bool HasEnoughMana()
    {
      return _mana.GetMana() >= manaCost;
    }

    private void TargetAcquired(AbillityData abillityData)
    {
      if (abillityData.IsCancelled())
      {
        return;
      }

      CooldownStore.StartCooldown(this, GetCooldownTime());

      foreach (var filter in filterStrategy)
      {
        abillityData.SetTargets(filter.Filter(abillityData.GetTargets()));
      }

      _mana.UseMana(manaCost);
      foreach (var effect in effectStrategy)
      {
        effect.StartEffect(abillityData, EffectFinished);
      }
    }

    private void EffectFinished()
    {
      Debug.Log("Effect Finished");
    }
  }
}