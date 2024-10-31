using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
  public class EnemyHealthDisplay : MonoBehaviour
  {
    Fighter _fighter;

    private void Awake()
    {
      _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
    }

    private void Update()
    {
      if (_fighter.GetTarget() == null)
      {
        GetComponent<TextMeshProUGUI>().text = "N/A";
        return;
      }
      Health health = _fighter.GetTarget();
      GetComponent<TextMeshProUGUI>().text = $"Enemy: {health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}";
    }
  }
}