using System.Collections;
using System.Collections.Generic;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public Canvas playingCanvas;
    public Canvas adminPanelCanvas;
    public Canvas votingCanvas;
    public Button startTimerButton;
    public Button resumeButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartPlaying()
    {
        votingCanvas.enabled = false;
        adminPanelCanvas.enabled = (AmongUsGoSettings.singleton.useAsAdminPanel);
        playingCanvas.enabled = !adminPanelCanvas.enabled;
        ((CustomNetworkManager) NetworkManager.singleton).CallForResume();
    }
    
    public void StartVoting()
    {
        resumeButton.gameObject.SetActive(false);
        startTimerButton.gameObject.SetActive(true);
        playingCanvas.enabled = false;
        adminPanelCanvas.enabled = false;
        votingCanvas.enabled = true;

        foreach (var pd in FindObjectsOfType<PlayerData>())
        {
            if(pd.type==PlayerData.PlayerType.Player)
                pd.CallVote();
        }
    }

    public void OnStartTimer()
    {
        startTimerButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
    }
    
    public void GameEnd()
    {
        playingCanvas.enabled = false;
        votingCanvas.enabled = false;

        ((CustomNetworkManager) NetworkManager.singleton).GameEnd();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
