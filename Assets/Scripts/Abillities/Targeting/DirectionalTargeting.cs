using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Targeting
{
  [CreateAssetMenu(fileName = "Directional Targeting", menuName = "Abillities/Targeting/Directional Targeting", order = 0)]
  public class DirectionalTargeting : TargetingStrategy
  {
    [FormerlySerializedAs("_layerMask")] [SerializeField] LayerMask layerMask;
    [FormerlySerializedAs("_groundOffset")] [SerializeField] float groundOffset = 1f;

    public override void StartTargeting(AbillityData abillityData, Action finished)
    {
      RaycastHit hit;
      Ray ray = PlayerController.GetMouseRay();
      bool hasHit = Physics.Raycast(ray, out hit, 1000, layerMask);
      if (hasHit)
      {
        Vector3 targetPoint = hit.point;
        abillityData.SetTargetPoint(targetPoint + ray.direction * groundOffset / ray.direction.y);
      }

      finished();
    }
  }
}