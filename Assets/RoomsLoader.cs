using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomsLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // read from rooms file
        //var rooms = ReadFromRoomsFile();
        var rooms = new string[]
        {
            "Bathroom1", "Bathroom2", "Front Entry"
        };
        var options = new List<Dropdown.OptionData>();
        foreach (var r in rooms)
        {
            options.Add(new Dropdown.OptionData(r));
        }
        
        GetComponent<Dropdown>().options = options;
    }

    private string[] ReadFromRoomsFile()
    {
        TextAsset mytxtData=(TextAsset)Resources.Load("MyText");
        string txt=mytxtData.text;
        return txt.Split(',');
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
