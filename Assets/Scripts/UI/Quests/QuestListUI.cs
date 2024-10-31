using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] QuestItemUI questPrefab;
    QuestList _questList;
    
    // Start is called before the first frame update
    void Start()
    {
        _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        _questList.OnUpdate += Redraw;
        Redraw();
    }

    private void Redraw()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
        foreach (QuestStatus status in _questList.GetStatuses())
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(status);
        }
    }
}
