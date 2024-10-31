using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
  public class Fader : MonoBehaviour
  {
    CanvasGroup _canvasGroup;
    Coroutine _currentActiveFade = null;

    private void Awake()
    {
      _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOutImmediate()
    {
      _canvasGroup.alpha = 1;
    }

    public Coroutine FadeOut(float time)
    {
      return Fade(1, time);
    }

    public Coroutine FadeIn(float time)
    {
      return Fade(0, time);
    }

    public Coroutine Fade(float target, float time)
    {
      if (_currentActiveFade != null)
      {
        StopCoroutine(_currentActiveFade);
      }
      _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
      return _currentActiveFade;
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
      while (!Mathf.Approximately(_canvasGroup.alpha, target))
      {
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
        yield return null;
      }
    }
  }
}