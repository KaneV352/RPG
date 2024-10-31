using System;
using RPG.Utils;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.UI
{
  public class SaveLoadRowUI : MonoBehaviour
  {
    [FormerlySerializedAs("_name")] [SerializeField] private TMPro.TextMeshProUGUI name = null;
    [FormerlySerializedAs("_dateLastPlayed")] [SerializeField] private TMPro.TextMeshProUGUI dateLastPlayed = null;
    [FormerlySerializedAs("_playTime")] [SerializeField] private TMPro.TextMeshProUGUI playTime = null;
    [FormerlySerializedAs("_level")] [SerializeField] private TMPro.TextMeshProUGUI level = null;

    private LazyValue<SavingWrapper> _savingWrapper = null;

    private void Awake()
    {
      _savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
    }

    private void Start()
    {
      _savingWrapper.ForceInit();
    }

    public void Setup(string name, string dateLastPlayed = "30/04/1975", string playTime = "999 Hr", string level = "99")
    {
      this.name.text = name;
      // _dateLastPlayed.text = dateLastPlayed;
      // _playTime.text = playTime;
      // _level.text = level;
    }

    public void Load()
    {
      Time.timeScale = 1;
      _savingWrapper.value.LoadGame(name.text);
    }

    public void Delete()
    {
      _savingWrapper.value.Delete(name.text);
      Destroy(gameObject);
    }
  }
}