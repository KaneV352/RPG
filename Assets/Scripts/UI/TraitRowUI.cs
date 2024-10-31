using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI
{
  public class TraitRowUI : MonoBehaviour
  {
    [FormerlySerializedAs("_trait")] [SerializeField] private Trait trait;
    [FormerlySerializedAs("_traitValue")] [SerializeField] private TextMeshProUGUI traitValue;
    [FormerlySerializedAs("_minusButton")] [SerializeField] private Button minusButton;
    [FormerlySerializedAs("_plusButton")] [SerializeField] private Button plusButton;

    private TraitStore _traitStore;

    private void OnEnable()
    {
      minusButton.onClick.AddListener(() => Allocate(-1));
      plusButton.onClick.AddListener(() => Allocate(1));
    }

    private void OnDisable()
    {
      minusButton.onClick.RemoveAllListeners();
      plusButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
      _traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
      traitValue.text = _traitStore.GetProposedPoints(trait).ToString();
    }

    private void Update()
    {
      minusButton.interactable = _traitStore.CanAssignPoints(trait, -1);
      plusButton.interactable = _traitStore.CanAssignPoints(trait, +1);

      traitValue.text = _traitStore.GetProposedPoints(trait).ToString();
    }

    private void Allocate(int point)
    {
      _traitStore.AssignPoints(trait, point);
    }
  }
}