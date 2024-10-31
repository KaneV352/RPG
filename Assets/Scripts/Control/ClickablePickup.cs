using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Control
{
  [RequireComponent(typeof(Pickup))]
  public class ClickablePickup : MonoBehaviour, IRaycastable
  {
    [FormerlySerializedAs("_pickupRange")] [SerializeField] private float pickupRange = 1f;
    Pickup _pickup;

    private void Awake()
    {
      _pickup = GetComponent<Pickup>();
    }

    private bool IsInRange(PlayerController callingController)
    {
      return Vector3.Distance(callingController.transform.position, transform.position) <= pickupRange;
    }

    public CursorType GetCursorType()
    {
      if (_pickup.CanBePickedUp())
      {
        return CursorType.Pickup;
      }
      else
      {
        return CursorType.FullPickup;
      }
    }

    public bool HandleRaycast(PlayerController callingController)
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (IsInRange(callingController))
        {
          _pickup.PickupItem();
        }
        else
        {
          StartCoroutine(MoveToPickup(callingController));
        }
      }
      return true;
    }

    private IEnumerator MoveToPickup(PlayerController callingController)
    {
      callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
      yield return new WaitUntil(() => Vector3.Distance(callingController.transform.position, transform.position) <= pickupRange);

      _pickup.PickupItem();
    }

  }
}
