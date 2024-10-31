using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using RPG.Utils;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    [FormerlySerializedAs("_chaseDistance")] [SerializeField] private float chaseDistance = 5f;
    [FormerlySerializedAs("_suspicionTime")] [SerializeField] private float suspicionTime = 3f;
    [FormerlySerializedAs("_aggroCooldownTime")] [SerializeField] private float aggroCooldownTime = 5f;
    [FormerlySerializedAs("_patrolPath")] [SerializeField] private PatrolPath patrolPath;
    [FormerlySerializedAs("_waypointTolerance")] [SerializeField] private float waypointTolerance = 1f;
    [FormerlySerializedAs("_waypointDwellTime")] [SerializeField] private float waypointDwellTime = 3f;
    [FormerlySerializedAs("_patrolSpeedFraction")]
    [Range(0, 1)]
    [SerializeField] private float patrolSpeedFraction = 0.2f;
    [FormerlySerializedAs("_shoutDistance")] [SerializeField] private float shoutDistance = 5f;

    private Fighter _fighter;
    private Health _health;
    private Mover _mover;
    private GameObject _player;

    private LazyValue<Vector3> _guardPosition;
    private float _timeSinceLastSawPlayer = Mathf.Infinity;
    private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
    private float _timeSinceAggrevated = Mathf.Infinity;
    private int _currentWaypointIndex = 0;

    private void Awake()
    {
      _fighter = GetComponent<Fighter>();
      _health = GetComponent<Health>();
      _mover = GetComponent<Mover>();
      _player = GameObject.FindWithTag("Player");

      _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
      _guardPosition.ForceInit();
    }

    private Vector3 GetGuardPosition()
    {
      return transform.position;
    }

    private void Update()
    {
      if (_health.IsDead()) return;

      if (IsAggrevated() && _fighter.CanAttack(_player))
      {
        AttackBehaviour();
      }
      else if (_timeSinceLastSawPlayer < suspicionTime)
      {
        SuspicionBehaviour();
      }
      else
      {
        PatrolBehaviour();
      }

      UpdateTimers();
    }

    public void Aggrevate()
    {
      _timeSinceAggrevated = 0;
    }

    public void Reset()
    {
      GetComponent<NavMeshAgent>().Warp(_guardPosition.value);
      _timeSinceAggrevated = Mathf.Infinity;
      _timeSinceLastSawPlayer = Mathf.Infinity;
      _timeSinceArrivedAtWaypoint = Mathf.Infinity;
      _currentWaypointIndex = 0;
    }

    private void UpdateTimers()
    {
      _timeSinceLastSawPlayer += Time.deltaTime;
      _timeSinceArrivedAtWaypoint += Time.deltaTime;
      _timeSinceAggrevated += Time.deltaTime;
    }

    private void PatrolBehaviour()
    {
      Vector3 nextPosition = _guardPosition.value;

      if (patrolPath != null)
      {
        if (AtWaypoint())
        {
          _timeSinceArrivedAtWaypoint = 0;
          CycleWaypoint();
        }
        nextPosition = GetCurrentWaypoint();
      }

      if (_timeSinceArrivedAtWaypoint > waypointDwellTime)
      {
        _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
      }
    }

    private bool AtWaypoint()
    {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
      return distanceToWaypoint < waypointTolerance;
    }

    private void CycleWaypoint()
    {
      _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
    }

    private Vector3 GetCurrentWaypoint()
    {
      return patrolPath.GetWaypoint(_currentWaypointIndex);
    }

    private void SuspicionBehaviour()
    {
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void AttackBehaviour()
    {
      _timeSinceLastSawPlayer = 0;
      _fighter.Attack(_player);

      AggrevateNearbyEnemies();
    }

    private void AggrevateNearbyEnemies()
    {
      RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
      foreach (RaycastHit hit in hits)
      {
        AIController ai = hit.collider.GetComponent<AIController>();
        if (ai == null) continue;

        ai.Aggrevate();
      }
    }

    private bool IsAggrevated()
    {
      float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
      return distanceToPlayer < chaseDistance || _timeSinceAggrevated < aggroCooldownTime;
    }

    // Called by Unity
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}