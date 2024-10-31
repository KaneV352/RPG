using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
  public class Projectile : MonoBehaviour
  {
    [SerializeField] float speed = 1;
    [SerializeField] bool isHoming = true;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] float maxLifeTime = 10;
    [SerializeField] GameObject[] destroyOnHit = null;
    [SerializeField] float lifeAfterImpact = 2;
    [SerializeField] UnityEvent onHit;

    Health _target = null;
    private Vector3 _targetPoint;

    GameObject _instigator = null;
    float _damage = 0;

    private void Start()
    {
      transform.LookAt(GetAimLocation());
    }

    void Update()
    {
      if (_target != null && isHoming && !_target.IsDead())
      {
        transform.LookAt(GetAimLocation());
      }
      transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetTarget(GameObject instigator, float damage, Health target)
    {
      SetTarget(instigator, damage, target, _targetPoint = default);
    }

    public void SetTarget(Vector3 targetPosition, GameObject instigator, float damage)
    {
      SetTarget(instigator, damage, null, targetPosition);
    }

    public void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)
    {
      this._target = target;
      this._targetPoint = targetPoint;
      this._damage = damage;
      this._instigator = instigator;

      Destroy(gameObject, maxLifeTime);
    }

    private Vector3 GetAimLocation()
    {
      if (_target == null)
      {
        return _targetPoint;
      }

      CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
      if (targetCapsule == null)
      {
        return _target.transform.position;
      }
      return _target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
      Health otherHealth = other.GetComponent<Health>();
      if (otherHealth == null) return;

      if (other.gameObject == _instigator) return;

      if (_target != null && otherHealth != _target) return;
      if (otherHealth == null || otherHealth.IsDead()) return;

      otherHealth.TakeDamage(_instigator, _damage);

      speed = 0;

      onHit.Invoke();

      if (hitEffect != null)
      {
        Instantiate(hitEffect, GetAimLocation(), transform.rotation);
      }

      foreach (GameObject toDestroy in destroyOnHit)
      {
        Destroy(toDestroy);
      }

      Destroy(gameObject, lifeAfterImpact);

    }

  }

}