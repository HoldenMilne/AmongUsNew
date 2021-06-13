using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataDesigner : MonoBehaviour
{
    private PlayerData data;

    public void SetPlayerReady(bool ready)
    {
        try
        {
            data.ready = ready;
        }catch (Exception e)
        {
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<PlayerData>();
    }

    public void SetPlayerName(InputField text)
    {
        var t = text.text;
        if (t.Equals(""))
        {
            t = "UNNAMED";
        }

        Debug.Log(t);
        /*
        try
        {
            data.CmdSetPlayerName(t);
        }catch (Exception e)
        {
            
        }*/
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
