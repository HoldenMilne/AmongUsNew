
using System.Collections.Generic;
using StationsAndHubs.Scripts.GameTasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadTasksToDropdown : MonoBehaviour
{
    private Dropdown dd;
    // Start is called before the first frame update
    void Start()
    {
        dd = GetComponent<Dropdown>();
        ReloadTaskLists();
    }

    public void ReloadTaskLists()
    {
        string[] listOfTasks = GameTask.taskFiles;
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>(listOfTasks.Length);
        foreach (var taskList in listOfTasks)
        {
            options.Add(new Dropdown.OptionData(taskList));
        }
        dd.options = options;
        dd.RefreshShownValue();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
