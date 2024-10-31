using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;
using RPG.Inventories;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction, ISaveable
  {
    private static readonly int Attack1 = Animator.StringToHash("attack");
    private static readonly int StopAttack1 = Animator.StringToHash("stopAttack");
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;
    [SerializeField] WeaponConfig defaultWeapon = null;

    Health _target;
    Equipment _equipment;
    float _timeSinceLastAttack = Mathf.Infinity;
    WeaponConfig _currentWeaponConfig;
    LazyValue<Weapon> _currentWeapon;

    private void Awake()
    {
      _currentWeaponConfig = defaultWeapon;
      _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
      _equipment = GetComponent<Equipment>();
      if (_equipment)
      {
        _equipment.EquipmentUpdated += UpdateWeapon;
      }
    }

    private Weapon SetupDefaultWeapon()
    {
      return AttachWeapon(defaultWeapon);
    }

    private void Start()
    {
      _currentWeapon.ForceInit();
    }

    private void Update()
    {
      _timeSinceLastAttack += Time.deltaTime;

      if (!_target) return;
      if (_target.IsDead()) return;

      if (!GetIsInRange(_target.transform))
      {
        GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
      }
      else
      {
        GetComponent<Mover>().Cancel();
        AttackBehaviour();
      }
    }

    public void EquipWeapon(WeaponConfig weapon)
    {
      _currentWeaponConfig = weapon;
      _currentWeapon.value = AttachWeapon(weapon);
    }

    private void UpdateWeapon()
    {
      var weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
      if (!weapon)
      {
        EquipWeapon(defaultWeapon);
      }
      else
      {
        EquipWeapon(weapon);
      }
    }

    private Weapon AttachWeapon(WeaponConfig weapon)
    {
      Animator animator = GetComponent<Animator>();
      return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
    }

    public Health GetTarget()
    {
      return _target;
    }

    private void AttackBehaviour()
    {
      transform.LookAt(_target.transform);
      if (_timeSinceLastAttack > timeBetweenAttacks)
      {
        // This will trigger the Hit() event.
        TriggerAttack();
        _timeSinceLastAttack = 0;
      }
    }

    private void TriggerAttack()
    {
      GetComponent<Animator>().ResetTrigger(StopAttack1);
      GetComponent<Animator>().SetTrigger(Attack1);
    }

    // Animation Event
    void Hit()
    {
      if (_target == null) { return; }

      float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

      if (_currentWeapon.value != null)
      {
        _currentWeapon.value.OnHit();
      }

      if (_currentWeaponConfig.HasProjectile())
      {
        _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
      }
      else
      {
        _target.TakeDamage(gameObject, damage);
      }
    }

    void Shoot()
    {
      Hit();
    }

    private bool GetIsInRange(Transform targetTransform)
    {
      return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetRange();
    }

    public bool CanAttack(GameObject combatTarget)
    {
      if (!combatTarget) { return false; }
      if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
          !GetIsInRange(combatTarget.transform))
      {
        return false;
      }
      Health targetToTest = combatTarget.GetComponent<Health>();
      return targetToTest && !targetToTest.IsDead();
    }

    public void Attack(GameObject combatTarget)
    {
      GetComponent<ActionScheduler>().StartAction(this);
      _target = combatTarget.GetComponent<Health>();
    }

    public void Cancel()
    {
      StopAttack();
      _target = null;
      GetComponent<Mover>().Cancel();
    }

    private void StopAttack()
    {
      GetComponent<Animator>().ResetTrigger(Attack1);
      GetComponent<Animator>().SetTrigger(StopAttack1);
    }

    public Transform GetHandTransform(bool isRightHanded)
    {
      if (isRightHanded)
      {
        return rightHandTransform;
      }
      else
      {
        return leftHandTransform;
      }
    }

    public object CaptureState()
    {
      return _currentWeaponConfig.name;
    }

    public void RestoreState(object state)
    {
      string weaponName = (string)state;
      WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
      EquipWeapon(weapon);
    }
  }
}