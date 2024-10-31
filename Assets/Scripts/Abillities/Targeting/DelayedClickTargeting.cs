using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Targeting
{
  [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abillities/Targeting/Delayed Click Targeting", order = 0)]
  public class DelayedClickTargeting : TargetingStrategy
  {
    [FormerlySerializedAs("_cursorTexture")] [SerializeField] private Texture2D cursorTexture;
    [FormerlySerializedAs("_cursorHotspot")] [SerializeField] private Vector2 cursorHotspot;
    [FormerlySerializedAs("_layerMask")] [SerializeField] private LayerMask layerMask;
    [FormerlySerializedAs("_areaAffectRadius")] [SerializeField] private float areaAffectRadius;
    [FormerlySerializedAs("_targetingPrefab")] [SerializeField] private Transform targetingPrefab;

    private Transform _targetingPrefabInstance = null;

    public override void StartTargeting(AbillityData abillityData, Action finished)
    {
      abillityData.GetUser().GetComponent<PlayerController>().StartCoroutine(Targeting(abillityData, finished));
    }

    private IEnumerator Targeting(AbillityData abillityData, Action finished)
    {
      PlayerController playerController = abillityData.GetUser().GetComponent<PlayerController>();
      if (playerController == null) yield break;

      playerController.enabled = false;
      if (_targetingPrefabInstance == null)
      {
        _targetingPrefabInstance = Instantiate(targetingPrefab);
      }
      else
      {
        _targetingPrefabInstance.gameObject.SetActive(true);
      }
      _targetingPrefabInstance.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);

      while (!abillityData.IsCancelled())
      {
        Cursor.visible = false;
        bool hasHit = Physics.Raycast(PlayerController.GetMouseRay(), out RaycastHit raycastHit, 1000, layerMask);
        if (hasHit)
        {
          _targetingPrefabInstance.position = raycastHit.point;

          if (Input.GetMouseButtonDown(0))
          {
            yield return new WaitWhile(() => { return Input.GetMouseButton(0); });

            abillityData.SetTargets(GetGameObjectsInRadius(raycastHit.point));
            abillityData.SetTargetPoint(raycastHit.point);
            break;
          }
        }
        if (Input.GetMouseButtonDown(1))
        {
          abillityData.Cancel();
          break;
        }

        yield return null;
      }
      _targetingPrefabInstance.gameObject.SetActive(false);
      Cursor.visible = true;
      playerController.enabled = true;

      finished();
    }

    private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
    {
      RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);

      foreach (var hit in hits)
      {
        yield return hit.collider.gameObject;
      }
    }
  }
}