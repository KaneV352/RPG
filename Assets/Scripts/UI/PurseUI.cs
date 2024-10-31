using RPG.Inventories;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.UI
{
  public class PurseUI : MonoBehaviour
  {
    [FormerlySerializedAs("_balanceText")] [SerializeField] private TMPro.TextMeshProUGUI balanceText = null;

    private Purse _purse = null;

    private void Start()
    {
      GameObject.FindGameObjectWithTag("Player").TryGetComponent(out _purse);

      if (_purse == null)
      {
        Debug.LogError("No Purse found in the scene.");
      }

      _purse.OnBalanceChanged += UpdateUI;

      UpdateUI();
    }

    private void UpdateUI()
    {
      balanceText.text = $"{_purse.GetBalance():N2}";
    }
  }
}