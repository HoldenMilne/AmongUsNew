using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    private static GameObject selected;
    
    public void Boot()
    {
        if (selected != null)
        {
            var net = FindObjectOfType<CustomNetworkManager>();
            var p = net.GetPlayer(selected.GetComponentInChildren<Text>().text);
            if(p!=null)
                p.connectionToClient.Disconnect();
        }
    }

    public void OnClick()
    {
        Debug.Log("CLICK");
    }

}
