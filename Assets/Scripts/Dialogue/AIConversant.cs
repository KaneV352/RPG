using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

namespace RPG.Dialogue
{
  public class AIConversant : MonoBehaviour, IRaycastable
  {
    [SerializeField] Dialogue dialogue  = null;
    [SerializeField] float maximumTalkingDistance = 1f;
    
    private PlayerConversant _playerConversant;
    private Mover _playerMover;

    private void Start()
    {
      _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
      _playerMover = _playerConversant.GetComponent<Mover>();
    }

    public CursorType GetCursorType()
    {
      return CursorType.Dialogue;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
      if (!dialogue) return false;
      
      if (Input.GetMouseButtonDown(0))
      {
        if (Vector3.Distance(callingController.transform.position, transform.position) > maximumTalkingDistance)
        {
          StartCoroutine(GetInRangeToTalk());
          return true;
        }
        else
        {
          _playerConversant.StartConversation(this ,dialogue);
          return true;
        }
      }

      return false;
    }

    private IEnumerator GetInRangeToTalk()
    {
      _playerMover.StartMoveAction(transform.position, 1f);
      
      yield return new WaitUntil( () => Vector3.Distance(_playerMover.transform.position, transform.position) <= maximumTalkingDistance);
      
      _playerConversant.StartConversation(this, dialogue);
    }
  }
}