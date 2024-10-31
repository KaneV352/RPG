using RPG.UI;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI
{
  public class TraitUI : MonoBehaviour
  {
    [FormerlySerializedAs("_unassignedValue")] [SerializeField] private TextMeshProUGUI unassignedValue;
    [FormerlySerializedAs("_comfirmButton")] [SerializeField] private Button comfirmButton;
    [FormerlySerializedAs("_cancelButton")] [SerializeField] private Button cancelButton;

    private TraitStore _traitStore;

    private void Start()
    {
      _traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
      ShowHideUI showHideUI = GetComponent<ShowHideUI>();

      comfirmButton.onClick.AddListener(_traitStore.CommitAssignment);
      cancelButton.onClick.AddListener(_traitStore.CancelAssignment);

      comfirmButton.onClick.AddListener(showHideUI.Exit);
      cancelButton.onClick.AddListener(showHideUI.Exit);
    }

    private void Update()
    {
      unassignedValue.text = _traitStore.GetUnassignedPoints().ToString();
    }
  }
}