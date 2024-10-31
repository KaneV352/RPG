using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
  public class HealthDisplay : MonoBehaviour
  {
    Health _health;

    private void Awake()
    {
      _health = GameObject.FindWithTag("Player").GetComponent<Health>();
    }

    private void Update()
    {
      GetComponent<TextMeshProUGUI>().text = $"Health: {_health.GetHealthPoints():0}/{_health.GetMaxHealthPoints():0}";
    }
  }
}