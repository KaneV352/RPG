using System;
using RPG.Saving;
using RPG.UI;
using RPG.Utils;
using RPG.Control;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI
{
  public class PauseMenuUI : MonoBehaviour
  {
    [FormerlySerializedAs("_resumeButton")] [SerializeField] private Button resumeButton = null;
    [FormerlySerializedAs("_saveButton")] [SerializeField] private Button saveButton = null;
    [FormerlySerializedAs("_loadButton")] [SerializeField] private Button loadButton = null;
    [FormerlySerializedAs("_optionsButton")] [SerializeField] private Button optionsButton = null;
    [FormerlySerializedAs("_exitButton")] [SerializeField] private Button exitButton = null;

    private LazyValue<PlayerController> _playerController;
    private LazyValue<SavingWrapper> _savingWrapper;

    void Awake()
    {
      _playerController = new LazyValue<PlayerController>(() => GameObject.FindWithTag("Player").GetComponent<PlayerController>());
      _savingWrapper = new LazyValue<SavingWrapper>(() => GameObject.FindObjectOfType<SavingWrapper>());
    }

    private void OnEnable()
    {
      if (_playerController.value == null)
      {
        _playerController.ForceInit();
      }
      if (_savingWrapper.value == null)
      {
        _savingWrapper.ForceInit();
      }

      _playerController.value.enabled = false;
      Time.timeScale = 0;
    }

    private void OnDisable()
    {
      if (_playerController.value == null)
      {
        _playerController.ForceInit();
      }
      if (_savingWrapper.value == null)
      {
        _savingWrapper.ForceInit();
      }

      _playerController.value.enabled = true;
      Time.timeScale = 1;
    }

    public void Resume()
    {
      GetComponentInParent<ShowHideUI>().Exit();
    }

    public void Save()
    {
      _savingWrapper.value.Save();
      Resume();
    }

    public void Load()
    {
      _savingWrapper.value.LoadGame();
      Resume();
    }

    public void Options()
    {
      Debug.Log("Options");
    }

    public void Exit()
    {
      _savingWrapper.value.ExitGame();
    }
  }
}