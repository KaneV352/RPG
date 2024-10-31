using RPG.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RPG.Attributes
{
  public class Health : MonoBehaviour, ISaveable
  {
    private static readonly int Die = Animator.StringToHash("die");
    [FormerlySerializedAs("_regenerationPercentage")] [SerializeField] private float regenerationPercentage = 70;
    [FormerlySerializedAs("_takeDamage")] [SerializeField] private TakeDamageEvent takeDamage;
    public UnityEvent onDie;

    [System.Serializable]
    public class TakeDamageEvent : UnityEvent<float>
    {
    }

    private LazyValue<float> _healthPoints;

    private bool _wasDeadLastFrame = false;

    private void Awake()
    {
      _healthPoints = new LazyValue<float>(GetInitialHealth);
    }

    private float GetInitialHealth()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void Start()
    {
      _healthPoints.ForceInit();
    }

    private void OnEnable()
    {
      GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
    }

    private void OnDisable()
    {
      GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
    }


    private void UpdateState()
    {
      Animator animator = GetComponent<Animator>();
      if (_wasDeadLastFrame && !IsDead())
      {
        animator.Rebind();
      }
      if (!_wasDeadLastFrame && IsDead())
      {
        animator.SetTrigger(Die);
        GetComponent<ActionScheduler>().CancelCurrentAction();
      }

      _wasDeadLastFrame = IsDead();
    }

    private void AwardExperience(GameObject instigator)
    {
      Experience experience = instigator.GetComponent<Experience>();
      if (!experience) return;

      experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
    }

    private void RegenerateHealth()
    {
      float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
      _healthPoints.value = Mathf.Max(_healthPoints.value, regenHealthPoints);
    }

    public bool IsDead()
    {
      return _healthPoints.value <= 0;
    }

    public void TakeDamage(GameObject instigator, float damage)
    {
      _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);

      if (IsDead())
      {
        onDie.Invoke();
        AwardExperience(instigator);
      }
      else
      {
        takeDamage.Invoke(damage);
      }
      UpdateState();
    }

    public void Heal(float healthToRestore)
    {
      _healthPoints.value = Mathf.Min(_healthPoints.value + healthToRestore, GetMaxHealthPoints());
      UpdateState();
    }

    public float GetHealthPoints()
    {
      return _healthPoints.value;
    }

    public float GetMaxHealthPoints()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    public float GetPercentage()
    {
      return 100 * GetFraction();
    }

    public float GetFraction()
    {
      return _healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    public object CaptureState()
    {
      return _healthPoints.value;
    }

    public void RestoreState(object state)
    {
      _healthPoints.value = (float)state;

      UpdateState();
    }
  }
}