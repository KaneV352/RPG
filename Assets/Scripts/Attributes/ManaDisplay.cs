using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
  public class ManaDisplay : MonoBehaviour
  {
    Mana _mana;

    private void Awake()
    {
      _mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
    }

    private void Update()
    {
      GetComponent<TextMeshProUGUI>().text = $"Mana: {_mana.GetMana():0}/{_mana.GetInitialMana():0}";
    }
  }
}