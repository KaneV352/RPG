using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.UI
{
  public class ShowHideUI : MonoBehaviour
  {
    [FormerlySerializedAs("_toggleKey")] [SerializeField] KeyCode toggleKey = KeyCode.Escape;
    [FormerlySerializedAs("_uiContainer")] [SerializeField] GameObject uiContainer = null;

    // Start is called before the first frame update
    void Start()
    {
      uiContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

      if (Input.GetKeyDown(toggleKey))
      {
        Toggle();
      }
    }

    public void Toggle()
    {
      uiContainer.SetActive(!uiContainer.activeSelf);
    }

    public void Exit()
    {
      uiContainer.SetActive(false);
    }
  }
}