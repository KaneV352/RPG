using System;
using RPG.Utils;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.UI
{
  public class SaveLoadUI : MonoBehaviour
  {
    [FormerlySerializedAs("_parent")] [SerializeField] private Transform parent = null;
    [FormerlySerializedAs("_saveLoadRowPrefab")] [SerializeField] private GameObject saveLoadRowPrefab = null;

    private LazyValue<SavingWrapper> _savingWrapper = null;

    private void Awake()
    {
      _savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
    }

    private void PopulateSaveSlots()
    {
      foreach (Transform child in parent)
      {
        Destroy(child.gameObject);
      }
      foreach (var fileName in _savingWrapper.value.GetSaveFiles())
      {
        var saveRow = Instantiate(saveLoadRowPrefab, parent);
        saveRow.GetComponent<SaveLoadRowUI>().Setup(fileName);
      }
    }

    private void OnEnable()
    {
      _savingWrapper.ForceInit();
      PopulateSaveSlots();
    }
  }
}