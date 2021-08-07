using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;

public class DataSync : MonoBehaviour
{
    public string PlayerName { get; set; }
    public PlayerData.PlayerType PlayerType { get; set; }

    public string playerName;
    public PlayerData.PlayerType playerType;

    public void Update()
    {
        playerName = PlayerName;
        playerType = PlayerType;
    }
}