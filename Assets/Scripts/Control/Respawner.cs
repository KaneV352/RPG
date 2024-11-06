using System;
using System.Collections;
using Unity.Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Control
{
  public class Respawner : MonoBehaviour
  {
    [FormerlySerializedAs("_respawnTime")][SerializeField] private float respawnTime = 3f;
    [FormerlySerializedAs("_healPercentage")]
    [Range(1, 100)]
    [SerializeField] private float healPercentage = 50f;
    [FormerlySerializedAs("_respawnPoint")][SerializeField] private Transform respawnPoint = null;
    [FormerlySerializedAs("_fadeTime")][SerializeField] private float fadeTime = 0.2f;
    [FormerlySerializedAs("_enemyHealPercentage")][SerializeField] private float enemyHealPercentage = 100f;

    private void Awake()
    {
      GetComponent<Health>().onDie.AddListener(Respawn);
    }

    private void Start()
    {
      if (GetComponent<Health>().IsDead())
      {
        Respawn();
      }
    }

    private void Respawn()
    {
      StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
      Fader fader = FindFirstObjectByType<Fader>();
      Health health = GetComponent<Health>();
      SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();

      savingWrapper.Save();
      yield return new WaitForSeconds(respawnTime);
      yield return fader.FadeOut(fadeTime);
      Respawn(health);
      ResetEnemy();
      savingWrapper.Save();
      yield return fader.FadeIn(fadeTime);
    }

    private void ResetEnemy()
    {
      foreach (AIController enemy in FindObjectsByType<AIController>(FindObjectsSortMode.None))
      {
        Health enemyHealth = enemy.GetComponent<Health>();
        if (!enemyHealth || enemyHealth.IsDead()) continue;
        enemy.Reset();
        enemyHealth.Heal(enemyHealth.GetMaxHealthPoints() * enemyHealPercentage / 100);
      }
    }

    private void Respawn(Health health)
    {
      Vector3 positionDelta = respawnPoint.position - transform.position;
      GetComponent<NavMeshAgent>().Warp(respawnPoint.position);
      transform.rotation = respawnPoint.rotation;
      health.Heal(health.GetMaxHealthPoints() * healPercentage / 100);
      CinemachineCamera activeVirtualCamera = FindFirstObjectByType<CinemachineBrain>().ActiveVirtualCamera as CinemachineCamera;
      if (activeVirtualCamera != null && activeVirtualCamera.Follow == transform)
      {
        activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta);
      }
    }
  }
}