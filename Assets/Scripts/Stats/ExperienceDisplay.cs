using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
  public class ExperienceDisplay : MonoBehaviour
  {
    Experience _experience;

    private void Awake()
    {
      _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
      if (_experience == null)
      {
        Debug.LogWarning("ExperienceDisplay: No Experience component found on player.");
      }
    }

    private void Update()
    {
      GetComponent<TextMeshProUGUI>().text = $"XP: {String.Format("{0:0}", _experience.GetPoints())}";
    }
  }
}