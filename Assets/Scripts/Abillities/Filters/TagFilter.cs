using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Abillities.Filters
{
  [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abillities/Filters/Tag Filter", order = 0)]
  public class TagFilter : FilterStrategy
  {
    [FormerlySerializedAs("_tag")] [SerializeField] private string tag;

    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectToFilters)
    {
      foreach (var gameObject in objectToFilters)
      {
        if (gameObject.CompareTag(tag))
        {
          yield return gameObject;
        }
      }
    }

  }
}