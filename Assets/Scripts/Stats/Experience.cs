using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
  public class Experience : MonoBehaviour, ISaveable
  {
    [SerializeField] float experiencePoints = 0;

    public event Action OnExperienceGained;

    public void GainExperience(float experience)
    {
      experiencePoints += experience;
      OnExperienceGained();
    }

    public float GetPoints()
    {
      return experiencePoints;
    }

    public object CaptureState()
    {
      return experiencePoints;
    }

    public void RestoreState(object state)
    {
      experiencePoints = (float)state;
    }
  }
}