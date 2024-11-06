using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.SceneManagement
{
  public class SavingWrapper : MonoBehaviour
  {
    public static string LastSaveFile = "_lastSaveFile";

    [FormerlySerializedAs("_fadeInTime")][SerializeField] private float fadeInTime = 0.2f;
    [FormerlySerializedAs("_fadeOutTime")][SerializeField] private float fadeOutTime = 0.2f;

    public void NewGame(string saveFile)
    {
      SetCurrentSaveFile(saveFile);
      StartCoroutine(StartNewGame(saveFile));
    }

    public void ContinueGame()
    {
      StartCoroutine(LoadLastScene());
    }

    public void LoadGame(string saveFile)
    {
      SetCurrentSaveFile(saveFile);
      StartCoroutine(LoadLastScene());
    }

    public void LoadGame()
    {
      StartCoroutine(LoadLastScene());
    }

    public void ExitGame()
    {
      StartCoroutine(LoadMainMenu());
    }


    private void SetCurrentSaveFile(string saveFile)
    {
      PlayerPrefs.SetString(LastSaveFile, saveFile);
    }

    private string GetCurrentSaveFile()
    {
      return PlayerPrefs.GetString(LastSaveFile);
    }

    private IEnumerator StartNewGame(string saveFile)
    {
      Fader fader = FindFirstObjectByType<Fader>();
      yield return fader.FadeOut(fadeOutTime);
      yield return GetComponent<SavingSystem>().LoadFirstScene(saveFile);
      yield return fader.FadeIn(fadeInTime);
    }

    private IEnumerator LoadLastScene()
    {
      Fader fader = FindFirstObjectByType<Fader>();
      yield return fader.FadeOut(fadeOutTime);
      yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSaveFile());
      yield return fader.FadeIn(fadeInTime);
    }

    private IEnumerator LoadMainMenu()
    {
      Fader fader = FindFirstObjectByType<Fader>();
      yield return fader.FadeOut(fadeOutTime);
      yield return GetComponent<SavingSystem>().LoadMainMenu();
      yield return fader.FadeIn(fadeInTime);
    }

    public void Load()
    {
      GetComponent<SavingSystem>().Load(GetCurrentSaveFile());
    }

    public void Save()
    {
      GetComponent<SavingSystem>().Save(GetCurrentSaveFile());
    }

    public void Delete(string saveFile)
    {
      GetComponent<SavingSystem>().Delete(saveFile);
    }

    public bool HasSaveFile()
    {
      return GetComponent<SavingSystem>().HasSaveFile(GetCurrentSaveFile());
    }

    public IEnumerable<string> GetSaveFiles()
    {
      return GetComponent<SavingSystem>().GetSaveFiles();
    }
  }
}