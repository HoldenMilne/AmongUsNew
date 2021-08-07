using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TEMP_UpdatePlayerName : MonoBehaviour
{
    public InputField input;

    private PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        if(playerData == null)
            foreach (var d in FindObjectsOfType<PlayerData>())
            {
                if (d.type == PlayerData.PlayerType.Player)
                {
                    var nid = d.gameObject.GetComponent<NetworkIdentity>();
                    if (nid.isLocalPlayer)
                    {
                        playerData = d;
                    }
                }
            }

        if (playerData == null) return;
        
        
        if (playerData.type == PlayerData.PlayerType.Player)
        {
            var nid = playerData.gameObject.GetComponent<NetworkIdentity>();
            if (nid.isLocalPlayer)
            {
                playerData.CmdSetPlayerName(playerData.connId,input.text);
            }
        }else if (playerData.type == PlayerData.PlayerType.Station)
        {
            var nid = playerData.gameObject.GetComponent<NetworkIdentity>();
            if (nid.isLocalPlayer)
            {
                playerData.CmdSetPlayerName(playerData.connId,input.text);
            }
        }
        
    }
    
    public void StartStationOnClick(Dropdown dropdown)
    {
        NetworkIdentity nid;
        if(playerData == null)
            foreach (var d in FindObjectsOfType<PlayerData>())
            {
                if (d.type == PlayerData.PlayerType.Station)
                {
                    nid = d.gameObject.GetComponent<NetworkIdentity>();
                    if (nid.isLocalPlayer)
                    {
                        playerData = d;
                    }
                }
            }

        Debug.Log(playerData);
        if (playerData == null) return;

        playerData.type = PlayerData.PlayerType.Station;

        var location = dropdown.options[dropdown.value].text;
        playerData.currentLocation = location;
        nid = playerData.gameObject.GetComponent<NetworkIdentity>();
        if (nid.isLocalPlayer)
        {
            if (playerData.isDataFirstSent)
                ;//playerData.Cmd(playerData.connId);
                
            else
                playerData.CmdRequestGameCode(playerData.connId,location);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
