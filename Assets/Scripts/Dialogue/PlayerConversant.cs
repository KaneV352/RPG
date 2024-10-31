using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Core;
using Random = System.Random;

namespace RPG.Dialogue
{
  public class PlayerConversant : MonoBehaviour
  {
    private Dialogue _currentDialogue;
    private DialogueNode _currentNode = null;
    private AIConversant _currentConversant = null;
    private bool _isChoosing = false;
    
    public event Action onConversantChange;

    public void StartConversation(AIConversant aiConversant, Dialogue dialogue)
    {
      _currentDialogue = dialogue;
      _currentNode = _currentDialogue.GetRootNode();
      _currentConversant = aiConversant;
      TriggerEntryAction();
      onConversantChange();
    }

    public string GetSpeakerName()
    {
      return "Irrigated Guard";
    }

    public bool IsChoosing()
    {
      return _isChoosing;
    }

    public bool IsSpeaking()
    {
      return _currentDialogue != null;
    }

    public void SelectChoice(DialogueNode dialogueNode)
    {
      _currentNode = dialogueNode;
      TriggerEntryAction();
      _isChoosing = false;
      
      Next();
    }

    public void Quit()
    {
      _currentDialogue = null;
      TriggerExitAction();
      _currentNode = null;
      _isChoosing = false;
      _currentConversant = null;

      onConversantChange();
    }

    public string GetText()
    {
      return _currentNode.GetText();
    }

    public IEnumerable<DialogueNode> GetChoices()
    {
      return _currentDialogue.GetPlayerChildren(_currentNode);
    }

    public void Next()
    {
      int numberPlayerChoices = _currentDialogue.GetPlayerChildren(_currentNode).Count();
      if (numberPlayerChoices > 0)
      {
        _isChoosing = true;
        onConversantChange();
        return;
      }
      else
      {
        var children = _currentDialogue.GetAIChildren(_currentNode).ToArray();
        int randomIndex = UnityEngine.Random.Range(0, children.Length);
        TriggerExitAction();
        _currentNode = children.ElementAt(randomIndex);
        TriggerEntryAction();
      }

      onConversantChange();
    }

    public bool HasNext()
    {
      var result = _currentDialogue.GetAllChildren(_currentNode);
      
      return result.Any();
    }
    
    private void TriggerEntryAction()
    {
      if (!_currentNode) return;
      if (_currentNode.GetOnEntryAction() == "") return;
      
      DialogueTrigger[] triggers = GetComponents<DialogueTrigger>();
      if (triggers.Length == 0) return;

      foreach (var trigger in triggers)
      {
        trigger.Trigger(_currentNode.GetOnEntryAction());
      }
    }
    
    private void TriggerExitAction()
    {
      if (!_currentNode) return;
      if (_currentNode.GetOnExitAction() == "") return;
      
      DialogueTrigger[] triggers = GetComponents<DialogueTrigger>();
      if (triggers.Length == 0) return;

      foreach (var trigger in triggers)
      {
        trigger.Trigger(_currentNode.GetOnExitAction());
      }
    }
  }
}