using System.Collections;
using System.Collections.Generic;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;

public class StartGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        ((CustomNetworkManager)NetworkManager.singleton).StartGame();
    }
    
}
