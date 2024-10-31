using System;
using UnityEngine;
using RPG.Combat;
using RPG.Attributes;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "Abillities/Effects/Spawn Projectile Effect", order = 0)]
  public class SpawnProjectileEffect : EffectStrategy
  {
    [FormerlySerializedAs("_projectilePrefab")] [SerializeField] private Projectile projectilePrefab = null;
    [FormerlySerializedAs("_damage")] [SerializeField] private float damage = 30f;
    [FormerlySerializedAs("_isRightHanded")] [SerializeField] private bool isRightHanded = true;
    [FormerlySerializedAs("_useTargetPoint")] [SerializeField] private bool useTargetPoint = true;

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      var fighter = abillityData.GetUser().GetComponent<Fighter>();
      var spawnPosition = fighter.GetHandTransform(isRightHanded).position;

      if (useTargetPoint)
      {
        SpawnProjectileForTargetPoint(abillityData, spawnPosition);
      }
      else
      {
        SpawnProjectilesForTargets(abillityData, spawnPosition);
      }

      finished();
    }

    private void SpawnProjectileForTargetPoint(AbillityData abillityData, Vector3 spawnPosition)
    {
      Vector3 targetPoint = abillityData.GetTargetPoint();

      Projectile projectile = Instantiate(projectilePrefab);
      projectile.transform.position = spawnPosition;
      projectile.SetTarget(abillityData.GetUser(), damage, null, targetPoint);
    }

    private void SpawnProjectilesForTargets(AbillityData abillityData, Vector3 spawnPosition)
    {
      foreach (var target in abillityData.GetTargets())
      {
        Health targetHealth = target.GetComponent<Health>();
        if (!targetHealth || targetHealth.IsDead()) continue;

        Projectile projectile = Instantiate(projectilePrefab);
        projectile.transform.position = spawnPosition;

        projectile.SetTarget(abillityData.GetUser(), damage, targetHealth);
      }
    }
  }
}