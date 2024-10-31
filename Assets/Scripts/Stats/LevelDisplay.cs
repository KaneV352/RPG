using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
  public class LevelDisplay : MonoBehaviour
  {
    BaseStats _baseStats;

    private void Awake()
    {
      _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
    }

    private void Update()
    {
      GetComponent<TextMeshProUGUI>().text = $"Level: {String.Format("{0:0}", _baseStats.GetLevel())}";
    }
  }
}