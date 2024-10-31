using System;
using RPG.Utils;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI
{
  public class MainMenuUI : MonoBehaviour
  {
    [FormerlySerializedAs("_inputField")] [SerializeField] private TMP_InputField inputField;
    [FormerlySerializedAs("_continueButton")] [SerializeField] private Button continueButton;
    [FormerlySerializedAs("_newGameConfirmButton")] [SerializeField] private Button newGameConfirmButton;

    private LazyValue<SavingWrapper> _savingWrapper;

    void Awake()
    {
      _savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
    }

    private void Start()
    {
      _savingWrapper.ForceInit();
    }

    private void Update()
    {
      continueButton.interactable = _savingWrapper.value.HasSaveFile();
      newGameConfirmButton.interactable = !String.IsNullOrWhiteSpace(inputField.text);
    }

    public void Continue()
    {
      _savingWrapper.value.ContinueGame();
    }

    public void NewGame()
    {
      _savingWrapper.value.NewGame(inputField.text);
    }

    public void Load()
    {
      Debug.Log("Load");
    }

    public void Exit()
    {
      Application.Quit();
    }
  }
}