using System;
using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Effects
{
  [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "Abillities/Effects/Spawn Target Prefab", order = 0)]
  public class SpawnTargetPrefabEffect : EffectStrategy
  {
    [FormerlySerializedAs("_prefabToSpawn")] [SerializeField] private Transform prefabToSpawn = null;
    [FormerlySerializedAs("_lifeTime")] [SerializeField] private float lifeTime = -1;

    public override void StartEffect(AbillityData abillityData, Action finished)
    {
      var prefabInstance = Instantiate(prefabToSpawn);
      prefabInstance.transform.position = abillityData.GetTargetPoint();

      abillityData.StartCoroutine(DestroyAfterTime(prefabInstance.gameObject));

      finished();
    }

    private IEnumerator DestroyAfterTime(GameObject gameObject)
    {
      if (!(lifeTime > 0)) yield break;
      yield return new WaitForSeconds(lifeTime);
      Destroy(gameObject);
    }
  }
}