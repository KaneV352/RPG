using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RPG.UI
{
  public class DialogueUI : MonoBehaviour
  {
    [SerializeField] Button quitButton;
    [SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] TextMeshProUGUI AIText;
    [SerializeField] Button nextButton;
    [SerializeField] GameObject AIResponse;
    [SerializeField] GameObject choiceRoot;
    [SerializeField] GameObject choiceButtonPrefab;

    private PlayerConversant _playerConversant;

    private void Start()
    {
      _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
      _playerConversant.onConversantChange += UpdateUI;

      nextButton.onClick.AddListener(() => _playerConversant.Next());
      quitButton.onClick.AddListener(() => _playerConversant.Quit());

      UpdateUI();
    }

    private void UpdateUI()
    {
      gameObject.SetActive(_playerConversant.IsSpeaking());

      if (!_playerConversant.IsSpeaking())
      {
        return;
      }

      speakerName.text = _playerConversant.GetSpeakerName();
      AIText.text = _playerConversant.GetText();
      nextButton.gameObject.SetActive(_playerConversant.HasNext());

      if (_playerConversant.IsChoosing())
      {
        AIResponse.SetActive(false);
        choiceRoot.SetActive(true);
        DrawChoices();
      }
      else
      {
        AIResponse.SetActive(true);
        choiceRoot.SetActive(false);
      }
    }

    private void DrawChoices()
    {
      foreach (Transform child in choiceRoot.transform)
      {
        Destroy(child.gameObject);
      }

      foreach (DialogueNode choice in _playerConversant.GetChoices())
      {
        var choiceButton = Instantiate(choiceButtonPrefab, choiceRoot.transform);
        choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
        choiceButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
          _playerConversant.SelectChoice(choice);
        });
      }
    }
  }
}