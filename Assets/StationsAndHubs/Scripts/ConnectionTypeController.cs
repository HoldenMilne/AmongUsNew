using System;
using System.Collections;
using System.Collections.Generic;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionTypeController : MonoBehaviour
{

    [HideInInspector]
    public bool lan;

    
    public CustomNetworkManager.ConnectionMethod type;

    public void Switch(Dropdown dropdown)
    {
        var s = dropdown.options[dropdown.value].text;
        Debug.Log(s);
        if(s.StartsWith("w",StringComparison.InvariantCultureIgnoreCase))
        {
            Debug.Log("WAN");
            lan = false;
            type = CustomNetworkManager.ConnectionMethod.WAN_PORT_FORWARDING;
            
            // show wan address input
        }
        else
        {
            Debug.Log("LAN");
            lan = true;
            type = CustomNetworkManager.ConnectionMethod.LAN;
            // hide wan address input
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetWanAddress()
    {
        return "";
    }
}
