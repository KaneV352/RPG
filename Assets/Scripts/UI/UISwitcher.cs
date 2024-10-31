using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.UI
{
  public class UISwitcher : MonoBehaviour
  {
    [FormerlySerializedAs("_startingPanel")] [SerializeField] private GameObject startingPanel = null;

    private void OnEnable()
    {
      if (startingPanel != null)
      {
        SwitchTo(startingPanel);
      }
    }

    public void SwitchTo(GameObject newPanel)
    {
      if (newPanel.transform.parent != transform) return;

      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(child.gameObject == newPanel);
      }
    }
  }
}