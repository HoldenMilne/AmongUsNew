using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskAdder : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject taskPrefab;
    public Sprite completeSprite;
    public Sprite incompleteSprite;


    private GameController gc;
    private List<GameObject> prefabs = new List<GameObject>();
    private void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTasks()
    {
        if (prefabs.Count > 0)
            prefabs = new List<GameObject>();
        float height = -1;
        int counter = 1;
        Debug.Log("ADDER"+gc.tasks);
        if(gc==null) return;
        
        foreach (var task in gc.tasks)
        {
            Debug.Log(task.ToString());
            var go = Instantiate(taskPrefab, gameObject.transform);
            var trigger = go.GetComponent<EventTrigger>();
            Text[] texts =  go.GetComponentsInChildren<Text>();
            if (texts[0].name.Equals("TaskName"))
            {
                texts[0].text = task.name;
                texts[1].text = task.location;
                trigger.triggers[0].callback.AddListener((eventData) => { LoadTaskGameOnDead(texts[0]); });

            }
            else
            {
                texts[0].text = task.location;
                texts[1].text = task.name;
                
                trigger.triggers[0].callback.AddListener((eventData) => { LoadTaskGameOnDead(texts[1]); });

            }
            var imgs = go.GetComponentsInChildren<Image>();

            foreach (var img in imgs)
            {
                if (img.name.Equals("Checkbox",StringComparison.InvariantCultureIgnoreCase))
                {
                    img.sprite = GetSprite(task);
                    break;
                }
            }

            if (height < 0)
            {
                height = go.GetComponent<RectTransform>().sizeDelta.y;
            }
            
            var rect = go.GetComponent<RectTransform>();
            var anc = rect.anchoredPosition;
            anc.y -= height * counter;
            rect.anchoredPosition = anc;
                

            counter += 1;
        }
    }

    void LoadTaskGameOnDead(Text t)
    {
        if (gc.isDead && FindObjectOfType<DeathPanelController>().timer<=0 && !gc.ghostsUseStationCode && !gc.winPanel.enabled)
        {
            foreach (var task in gc.tasks)
            {
                if (!task.isComplete && task.name.Equals(t.text)) // find the first incomplete task at that location.
                {
                    gc.currentTask = task;
                    gc.LoadScene(task.name);
                    return;
                }
            }
        }
    }

    private Sprite GetSprite(Task t)
    {
        if (t.isComplete)
        {
            return completeSprite;
        }
        return incompleteSprite;
    }

    public void SetTaskComplete(string currentTaskName)
    {
        foreach (GameObject go in prefabs)
        {
            Text[] texts =  go.GetComponentsInChildren<Text>();

            string s;
            if (texts[0].name.Equals("TaskName"))
            {
                s= texts[0].text;
            }
            else
            {
                s= texts[1].text;

            }
        }
    }
}
