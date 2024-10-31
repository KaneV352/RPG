using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abillities
{
  public class AbillityData : IAction
  {
    private GameObject _user;
    private Vector3 _targetPoint;
    private IEnumerable<GameObject> _targets;
    private bool _isCancelled = false;

    public bool IsCancelled()
    {
      return _isCancelled;
    }

    public AbillityData(GameObject user)
    {
      _user = user;
    }

    public void SetTargets(IEnumerable<GameObject> targets)
    {
      _targets = targets;
    }

    public IEnumerable<GameObject> GetTargets()
    {
      return _targets;
    }

    public GameObject GetUser()
    {
      return _user;
    }

    public void SetTargetPoint(Vector3 targetPoint)
    {
      _targetPoint = targetPoint;
    }

    public Vector3 GetTargetPoint()
    {
      return _targetPoint;
    }


    public void StartCoroutine(IEnumerator coroutine)
    {
      _user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }

    public void StartAction()
    {
      _isCancelled = false;
    }

    public void Cancel()
    {
      _isCancelled = true;
    }
  }
}